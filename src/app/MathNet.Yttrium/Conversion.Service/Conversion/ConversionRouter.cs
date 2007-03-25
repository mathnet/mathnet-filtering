#region Math.NET Yttrium (GPL) by Christoph Ruegg
// Math.NET Yttrium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MathNet.Symbolics.Conversion
{
    public class ConversionRouter : IValueConverter, IConversionRouter
    {
        private MathIdentifier structureId;
        private Dictionary<MathIdentifier, ConversionRoute> losslessNeighbors, lossyNeighbors;
        private Dictionary<MathIdentifier, IConversionRouter> targetNeighbors;
        private Dictionary<MathIdentifier, IRouteDistance> vector;

        #region Routing Structures
        private struct ConversionRoute
        {
            public ConversionRoute(MathIdentifier canConvertFrom, Converter<ICustomData, ICustomData> convert)
            {
                this.CanConvertFrom = canConvertFrom;
                this.Convert = convert;
            }
            public readonly MathIdentifier CanConvertFrom;
            public readonly Converter<ICustomData, ICustomData> Convert;
        }

        private struct ConversionDistance : IRouteDistance
        {
            private readonly MathIdentifier _canConvertFrom;
            private readonly int _cost;
            private readonly IConversionRouter _nextHop;
            private readonly Converter<ICustomData, ICustomData> _convert;
            private readonly bool _lossless;

            public ConversionDistance(MathIdentifier canConvertFrom, int cost, bool lossless, IConversionRouter nextHop, Converter<ICustomData, ICustomData> convert)
            {
                _canConvertFrom = canConvertFrom;
                _cost = cost;
                _lossless = lossless;
                _nextHop = nextHop;
                _convert = convert;
            }

            public MathIdentifier CanConvertFrom
            {
                get { return _canConvertFrom; }
            }
            public int Cost
            {
                get { return _cost; }
                //set { _cost = value; }
            }
            public IConversionRouter NextHop
            {
                get { return _nextHop; }
                //set { _nextHop = value; }
            }
            public Converter<ICustomData, ICustomData> Convert
            {
                get { return _convert; }
                //set { _convert = value; }
            }
            public bool Lossless
            {
                get { return _lossless; }
                //set { _lossless = value; }
            }
        }
        #endregion

        public ConversionRouter(MathIdentifier structureId)
        {
            this.structureId = structureId;
            this.losslessNeighbors = new Dictionary<MathIdentifier, ConversionRoute>();
            this.lossyNeighbors = new Dictionary<MathIdentifier, ConversionRoute>();
            this.targetNeighbors = new Dictionary<MathIdentifier, IConversionRouter>();
            this.vector = new Dictionary<MathIdentifier, IRouteDistance>();
        }

        public MathIdentifier TypeIdentifier
        {
            get { return structureId; }
        }

        #region Routing
        public void AddSourceNeighbor(IConversionRouter routerWeCanConvertFrom, bool lossless, Converter<ICustomData, ICustomData> directConvert)
        {
            MathIdentifier id = routerWeCanConvertFrom.TypeIdentifier;
            Propose(new ConversionDistance(id, 0, lossless, null, directConvert), routerWeCanConvertFrom);
        }

        public void AddTargetNeighbor(IConversionRouter routerWeCanConvertTo)
        {
            if(!targetNeighbors.ContainsKey(routerWeCanConvertTo.TypeIdentifier))
            {
                targetNeighbors.Add(routerWeCanConvertTo.TypeIdentifier, routerWeCanConvertTo);
                BroadcastDistanceVector(routerWeCanConvertTo);
            }
        }

        /// <returns>True if the route was better than existing routes.</returns>
        /// <remarks>It is expected that we already know the proposing neighbor router <c>proposedBy</c>!</remarks>
        public bool Propose(IRouteDistance distance, IConversionRouter proposedBy)
        {
            MathIdentifier id = distance.CanConvertFrom;

            // I'm already myself, so I won't accept it whatever you propose :)
            if(structureId.Equals(id))
                return false;

            bool knownLosslessProposer = losslessNeighbors.ContainsKey(proposedBy.TypeIdentifier);
            bool knownLossyProposer = lossyNeighbors.ContainsKey(proposedBy.TypeIdentifier);

            // does the router propose himself?
            if(proposedBy.TypeIdentifier.Equals(id))
            {
                if(distance.Cost != 0)
                    throw new InvalidOperationException("An conversion router strangely thinks he can't reach himself with zero cost.");

                // we didn't know it, or it's better than before, so we add it to the neighbor list
                if(distance.Lossless && !knownLosslessProposer)
                    losslessNeighbors[id] = new ConversionRoute(id, distance.Convert);
                else if(!distance.Lossless && !knownLossyProposer)
                    lossyNeighbors[id] = new ConversionRoute(id, distance.Convert);
                else
                    return false;

                // we also want to add this neighbour to the vector.
                vector[id] = new ConversionDistance(id, 1, distance.Lossless, proposedBy, distance.Convert);
                
                // before we broadcast our vector, we wan't to "subscribe" on this new inbound router
                // to receive all his broadcasts.
                proposedBy.AddTargetNeighbor(this);

                // now we broadcast (probably we already did this when the router sent any
                // broadcasts as a result of AddTargetNeighbor, but we want to be sure)
                BroadcastDistanceVector();

                return true;
            }

            if(!(knownLosslessProposer || knownLossyProposer))
                throw new InvalidOperationException("An unknown router strangely proposes some other host without first proposing himself.");

            // apparently he proposes some other host to be routed through himself,
            // so we have to combine the poposed cost with those required to reach himself
            bool lossless = distance.Lossless && knownLosslessProposer;
            int cost = distance.Cost + 1;

            // is the proposed route better, or didn't we even know such a route yet?
            IRouteDistance existingDistance;
            if(!vector.TryGetValue(id, out existingDistance)
                || lossless && !existingDistance.Lossless
                || cost < existingDistance.Cost && (lossless || !existingDistance.Lossless))
            {
                // then we want to add this route to our vector

                // we have to map the proposed conversion with then one required to convert from him
                Converter<ICustomData, ICustomData> distanceC = distance.Convert, localC, convert;
                if(knownLosslessProposer)
                    localC = losslessNeighbors[proposedBy.TypeIdentifier].Convert;
                else
                    localC = lossyNeighbors[proposedBy.TypeIdentifier].Convert;
                convert = delegate(ICustomData v) { return localC(distanceC(v)); };

                // Add route to vector and broadcast the vector.
                vector[id] = new ConversionDistance(id, cost, lossless, proposedBy, convert);
                BroadcastDistanceVector();

                return true;
            }
            else
                return false;
        }
        public void Propose(Dictionary<MathIdentifier, IRouteDistance> vec, IConversionRouter proposedBy)
        {
            foreach(KeyValuePair<MathIdentifier, IRouteDistance> distance in vec)
                Propose(distance.Value, proposedBy);
        }

        public void BroadcastDistanceVector(IConversionRouter target)
        {
            target.Propose(vector, this);
        }
        public void BroadcastDistanceVector()
        {
            foreach(KeyValuePair<MathIdentifier, IConversionRouter> r in targetNeighbors)
                r.Value.Propose(vector, this);
        }
        #endregion

        #region Conversion
        public Converter<ICustomData, ICustomData> BuildConverterFrom(MathIdentifier id)
        {
            if(structureId.Equals(id))
                return delegate(ICustomData value) { return value; };
            IRouteDistance cd;
            if(vector.TryGetValue(id, out cd))
                return cd.Convert;

            throw new NotSupportedException("no route available to convert from " + id.ToString());
            //throw new MathNet.Symbolics.Backend.Exceptions.IncompatibleStructureException(id.Label, id.Domain);
        }

        public ICustomData ConvertFrom(ICustomData value)
        {
            MathIdentifier id = value.TypeId;
            if(structureId.Equals(id))
                return value;
            IRouteDistance cd;
            if(vector.TryGetValue(id,out cd))
                return cd.Convert(value);

            throw new NotSupportedException("no route available to convert from " + id.ToString());
            //throw new MathNet.Symbolics.Backend.Exceptions.IncompatibleStructureException(id.Label, id.Domain);
        }

        public bool CanConvertFrom(MathIdentifier id, bool allowLoss)
        {
            if(structureId.Equals(id))
                return true;
            IRouteDistance cd;
            if(vector.TryGetValue(id, out cd))
                return cd.Lossless || allowLoss;
            return false;
        }
        public bool CanConvertLosslessFrom(ICustomData value)
        {
            return value != null && CanConvertFrom(value.TypeId, false);
        }
        public bool CanConvertLossyFrom(ICustomData value)
        {
            return value != null && CanConvertFrom(value.TypeId, true);
        }

        public bool TryConvertFrom(ICustomData value, out ICustomData returnValue)
        {
            MathIdentifier id = value.TypeId;
            if(structureId.Equals(id))
            {
                returnValue = value;
                return true;
            }
            IRouteDistance cd;
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
            foreach(KeyValuePair<MathIdentifier,IRouteDistance> item in vector)
            {
                sb.Append(' '); sb.Append(item.Key.ToString());
                sb.Append(':'); sb.Append(item.Value.Cost);
                sb.Append(':'); sb.Append(item.Value.Lossless);
                sb.Append(':'); sb.Append(item.Value.NextHop.TypeIdentifier.ToString());
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
