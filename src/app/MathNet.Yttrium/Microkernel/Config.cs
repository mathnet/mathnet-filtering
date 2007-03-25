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
using System.Text;
using System.Globalization;
using System.Configuration;

//using log4net;

namespace MathNet.Symbolics
{
    // TODO: Does the log really belong into this config class? rather not -> move it to a seperate static Log class

    public static class Config
    {
        //private static ILog log;

        static Config()
        {
            //log4net.Config.XmlConfigurator.Configure();
            //log = LogManager.GetLogger("yttrium");
        }

        public static string YttriumNamespace
        {
            get { return @"http://www.cdrnet.net/projects/nmath/symbolics/yttrium/system/0.50/"; }
        }

        public static Guid GenerateInstanceId()
        {
            return Guid.NewGuid();
        }

        public static Encoding InternalEncoding
        {
            get { return Encoding.Unicode; }
        }

        public static CultureInfo InternalCulture
        {
            get { return CultureInfo.InvariantCulture; }
        }

        public static NumberFormatInfo InternalNumberFormat
        {
            get { return NumberFormatInfo.InvariantInfo; }
        }

        public static StringComparer IdentifierComparer
        {
            get { return StringComparer.InvariantCulture; }
        }

        //internal static Configuration LoadMappingConfig()
        //{
        //    ExeConfigurationFileMap mappingFileMap = new ExeConfigurationFileMap();
        //    mappingFileMap.ExeConfigFilename = @"yttrium.mapping.config";
        //    return ConfigurationManager.OpenMappedExeConfiguration(mappingFileMap, ConfigurationUserLevel.None);
        //}


        private static Random _random = new Random();
        [Obsolete] 
        public static int GenerateTag()
        {
            // TODO: Get rid of the outdated tagging subsystem
            return _random.Next();
        }

        #region Format Settings
        private static char separatorCharacter = ',';
        public static char SeparatorCharacter
        {
            get { return separatorCharacter; }
            set { separatorCharacter = value; }
        }

        private static char executorCharacter = ';';
        public static char ExecutorCharacter
        {
            get { return executorCharacter; }
            set { executorCharacter = value; }
        }

        private static EncapsulationFormat listEncapsulation = new EncapsulationFormat('(', ')');
        public static EncapsulationFormat ListEncapsulation
        {
            get { return listEncapsulation; }
            set { listEncapsulation = value; }
        }

        private static EncapsulationFormat vectorEncapsulation = new EncapsulationFormat('[', ']');
        public static EncapsulationFormat VectorEncapsulation
        {
            get { return vectorEncapsulation; }
            set { vectorEncapsulation = value; }
        }

        private static EncapsulationFormat setEncapsulation = new EncapsulationFormat('{', '}');
        public static EncapsulationFormat SetEncapsulation
        {
            get { return setEncapsulation; }
            set { setEncapsulation = value; }
        }

        private static EncapsulationFormat scalarEncapsulation = new EncapsulationFormat('<', '>');
        public static EncapsulationFormat ScalarEncapsulation
        {
            get { return scalarEncapsulation; }
            set { scalarEncapsulation = value; }
        }

        private static EncapsulationFormat literalEncapsulation = new EncapsulationFormat('"', '"');
        public static EncapsulationFormat LiteralEncapsulation
        {
            get { return literalEncapsulation; }
            set { literalEncapsulation = value; }
        }
        #endregion
    }
}
