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
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MathNet.Symbolics.Library
{
    internal sealed class XmlPackagesAdapter
    {
        #region Load from different XML sources
        public IList<PackageInfo> Load(string sourceFilename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(sourceFilename);

            return Load(xmlDoc);
        }

        public IList<PackageInfo> Load(XmlDocument sourceDoc)
        {
            XmlNode objectsSection = sourceDoc.SelectSingleNode("//yttrium/packages");
            if(objectsSection == null)
                return null; // no bindings found
            else
                return CompileFromXml(objectsSection);
        }
        #endregion

        #region Compile from XML elements
        internal IList<PackageInfo> CompileFromXml(XmlNode objectsSection)
        {
            List<PackageInfo> infos = new List<PackageInfo>();
            foreach(XmlNode packageDef in objectsSection.SelectNodes("package"))
            {
                PackageInfo info =
                    new PackageInfo(GetXmlAttribute(packageDef, "domain", ""),
                                   GetXmlAttribute(packageDef, "manager", ""),
                                   GetXmlAttribute(packageDef, "default", ""),
                                   GetXmlAttribute(packageDef, "comment", ""));
                infos.Add(info);
            }
            return infos;
        }

        private string GetXmlAttribute(XmlNode node, string attributeName, string defaultValue)
        {
            try
            {
                return node.Attributes[attributeName].Value;
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion
    }
}
