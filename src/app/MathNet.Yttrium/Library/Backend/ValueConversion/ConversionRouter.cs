#region Copyright 2001-2006 Christoph Daniel Rüegg [GPL]
//Math.NET Symbolics: Yttrium, part of Math.NET
//Copyright (c) 2001-2006, Christoph Daniel Rueegg, http://cdrnet.net/.
//All rights reserved.
//This Math.NET package is available under the terms of the GPL.

//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.ValueConversion
{
    public class ConversionRouter
    {
        private MathIdentifier structureId;
        private Dictionary<MathIdentifier, ConversionRoute> losslessNeighbors, lossyNeighbors;
        private Dictionary<MathIdentifier, ConversionRouter> targetNeighbors;
        private Dictionary<MathIdentifier, ConversionDistance> vector;

        #region Routing Structures
        private struct ConversionRoute
        {
            public ConversionRoute(MathIdentifier canConvertFrom, Converter<ValueStructure, ValueStructure> convert)
            {
                this.CanConvertFrom = canConvertFrom;
                this.Convert = convert;
            }
            public MathIdentifier CanConvertFrom;
            public Converter<ValueStructure, ValueStructure> Convert;
        }

        private struct ConversionDistance
        {
            public ConversionDistance(MathIdentifier canConvertFrom, int cost, bool lossless, ConversionRouter nextHop, Converter<ValueStructure, ValueStructure> convert)
            {
                this.CanConvertFrom = canConvertFrom;
                this.Cost = cost;
                this.Lossless = lossless;
                this.NextHop = nextHop;
                this.Convert = convert;
            }
            public MathIdentifier CanConvertFrom;
            public int Cost;
            public ConversionRouter NextHop;
            public Converter<ValueStructure, ValueStructure> Convert;
            public bool Lossless;
        }
        #endregion

        public ConversionRouter(MathIdentifier structureId)
        {
            this.structureId = structureId;
            this.losslessNeighbors = new Dictionary<MathIdentifier, ConversionRoute>();
            this.lossyNeighbors = new Dictionary<MathIdentifier, ConversionRoute>();
            this.targetNeighbors = new Dictionary<MathIdentifier, ConversionRouter>();
            this.vector = new Dictionary<MathIdentifier, ConversionDistance>();
        }

        public MathIdentifier StructureIdentifier
        {
            get { return structureId; }
        }

        #region Routing
        public void AddSourceNeighbor(ConversionRouter routerWeCanConvertFrom, bool lossless, Converter<ValueStructure, ValueStructure> directConvert)
        {
            MathIdentifier id = routerWeCanConvertFrom.structureId;
            if(Propose(new ConversionDistance(id, 0, lossless, null, directConvert), routerWeCanConvertFrom))
            {
                ConversionRoute route = new ConversionRoute(id, directConvert);
                if(lossless)
                {
                    if(losslessNeighbors.ContainsKey(id))
                        losslessNeighbors[id] = route;
                    else
                        losslessNeighbors.Add(id, route);
                }
                else
                {
                    if(lossyNeighbors.ContainsKey(id))
                        lossyNeighbors[id] = route;
                    else
                        lossyNeighbors.Add(id, route);
                }
                routerWeCanConvertFrom.AddTargetNeighbor(this); //receiving all its routes ...
                BroadcastDistanceVector();
            }
        }

        private void AddTargetNeighbor(ConversionRouter routerWeCanConvertTo)
        {
            if(!targetNeighbors.ContainsKey(routerWeCanConvertTo.structureId))
            {
                targetNeighbors.Add(routerWeCanConvertTo.structureId, routerWeCanConvertTo);
                BroadcastDistanceVector(routerWeCanConvertTo);
            }
        }

        /// <returns>True if the route was better than existing routes.</returns>
        private bool Propose(ConversionDistance distance, ConversionRouter proposedBy)
        {
            MathIdentifier id = distance.CanConvertFrom;
            if(structureId.Equals(id))
                return false;
            distance.Lossless = distance.Lossless && losslessNeighbors.ContainsKey(proposedBy.structureId);
            distance.Cost++;
            if(distance.NextHop != null) //not a new neighbour
            {
                Converter<ValueStructure, ValueStructure> distanceC = distance.Convert, localC;
                if(losslessNeighbors.ContainsKey(proposedBy.structureId))
                    localC = losslessNeighbors[proposedBy.structureId].Convert;
                else
                    localC = lossyNeighbors[proposedBy.structureId].Convert;
                distance.Convert = delegate(ValueStructure v) { return localC(distanceC(v)); };
            }
            distance.NextHop = proposedBy;
            if(vector.ContainsKey(id))
            {
                ConversionDistance cd = vector[id];
                if(distance.Lossless && !cd.Lossless || distance.Cost < cd.Cost && (distance.Lossless || !cd.Lossless))
                {
                    vector[id] = distance;
                    BroadcastDistanceVector();
                    return true;
                }
                return false;
            }
            vector.Add(id, distance);
            BroadcastDistanceVector();
            return true;
        }
        private void Propose(Dictionary<MathIdentifier, ConversionDistance> vec, ConversionRouter proposedBy)
        {
            foreach(KeyValuePair<MathIdentifier, ConversionDistance> distance in vec)
                Propose(distance.Value, proposedBy);
        }

        public void BroadcastDistanceVector(ConversionRouter target)
        {
            target.Propose(vector, this);
        }
        public void BroadcastDistanceVector()
        {
            foreach(KeyValuePair<MathIdentifier, ConversionRouter> r in targetNeighbors)
                r.Value.Propose(vector, this);
        }
        #endregion

        #region Conversion
        public Converter<ValueStructure, ValueStructure> BuildConverterFrom(MathIdentifier id)
        {
            if(structureId.Equals(id))
                return delegate(ValueStructure value) { return value; };
            ConversionDistance cd;
            if(vector.TryGetValue(id, out cd))
                return cd.Convert;

            throw new MathNet.Symbolics.Backend.Exceptions.IncompatibleStructureException(id.Label, id.Domain);
        }

        public ValueStructure ConvertFrom(ValueStructure value)
        {
            MathIdentifier id = value.StructureId;
            if(structureId.Equals(id))
                return value;
            ConversionDistance cd;
            if(vector.TryGetValue(id,out cd))
                return cd.Convert(value);

            throw new MathNet.Symbolics.Backend.Exceptions.IncompatibleStructureException(id.Label, id.Domain);
        }

        public bool CanConvertFrom(MathIdentifier id, bool allowLoss)
        {
            if(structureId.Equals(id))
                return true;
            ConversionDistance cd;
            if(vector.TryGetValue(id, out cd))
                return cd.Lossless || allowLoss;
            return false;
        }
        public bool CanConvertLosslessFrom(ValueStructure value)
        {
            return value != null && CanConvertFrom(value.StructureId, false);
        }
        public bool CanConvertLossyFrom(ValueStructure value)
        {
            return value != null && CanConvertFrom(value.StructureId, true);
        }

        public bool TryConvertFrom(ValueStructure value, out ValueStructure returnValue)
        {
            MathIdentifier id = value.StructureId;
            if(structureId.Equals(id))
            {
                returnValue = value;
                return true;
            }
            ConversionDistance cd;
            if(vector.TryGetValue(id, out cd))
            {
                returnValue = cd.Convert(value);
                return true;
            }
            returnValue = null;
            return false;
        }
        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Routing "); sb.AppendLine(structureId.ToString());
            sb.Append("Lossless Neighbours:");
            foreach(MathIdentifier id in losslessNeighbors.Keys)
            { sb.Append(' '); sb.Append(id.ToString()); }
            sb.AppendLine();
            sb.Append("Lossy Neighbours:");
            foreach(MathIdentifier id in lossyNeighbors.Keys)
            { sb.Append(' '); sb.Append(id.ToString()); }
            sb.AppendLine();
            sb.Append("From:");
            foreach(KeyValuePair<MathIdentifier,ConversionDistance> item in vector)
            {
                sb.Append(' '); sb.Append(item.Key.ToString());
                sb.Append(':'); sb.Append(item.Value.Cost);
                sb.Append(':'); sb.Append(item.Value.Lossless);
                sb.Append(':'); sb.Append(item.Value.NextHop.StructureIdentifier.ToString());
            }
            sb.AppendLine();
            sb.Append("To:");
            foreach(MathIdentifier id in targetNeighbors.Keys)
            { sb.Append(' '); sb.Append(id.ToString()); }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
