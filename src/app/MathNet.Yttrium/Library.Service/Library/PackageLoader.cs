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
using System.IO;

namespace MathNet.Symbolics.Library
{
    public class PackageLoader : IPackageLoader
    {
        private static Dictionary<string, PackageInfo> _packagesOffline, _packagesOnline;
        private ILibrary _library = Service<ILibrary>.Instance;

        static PackageLoader()
        {
            _packagesOffline = new Dictionary<string, PackageInfo>();
            _packagesOnline = new Dictionary<string, PackageInfo>();
            LoadPackageInfo();
        }

        #region Load Available Package Infos
        public static void LoadPackageInfo()
        {
            IList<PackageInfo> infos
                = System.Configuration.ConfigurationManager.GetSection("yttrium/packages") as IList<PackageInfo>;
            if(infos == null)
            {
                FileInfo fi = new FileInfo(@"yttrium.packages.config");
                if(fi.Exists)
                {
                    XmlPackagesAdapter xpa = new XmlPackagesAdapter();
                    infos = xpa.Load(fi.FullName);
                }
            }
            if(infos != null)
                foreach(PackageInfo info in infos)
                    _packagesOffline.Add(info.domain, info);
        }
        public static void LoadPackageInfo(string sourceFilename)
        {
            XmlPackagesAdapter xpa = new XmlPackagesAdapter();
            IList<PackageInfo> infos = xpa.Load(sourceFilename);
            if(infos != null)
                foreach(PackageInfo info in infos)
                    _packagesOffline.Add(info.domain, info);
        }
        #endregion

        private void LoadPackage(PackageInfo info)
        {
            _packagesOnline.Add(info.domain, info);
            _packagesOffline.Remove(info.domain);
            IPackageManager pm = (IPackageManager)info.managerType
                    .GetConstructor(new Type[] { })
                    .Invoke(new object[] { });
            pm.Register(_library);
        }

        public void LoadPackage(string domain)
        {
            PackageInfo info;
            if(_packagesOffline.TryGetValue(domain, out info))
                LoadPackage(info);
        }

        public void UnloadPackage(string domain)
        {
            throw new NotImplementedException();
        }

        public void ReloadPackage(string domain)
        {
            UnloadPackage(domain);
            LoadPackage(domain);
        }

        public void LoadStdPackage()
        {
            LoadPackage("Std");
        }

        public void UnloadStdPackage()
        {
            UnloadPackage("Std");
        }

        public void ReloadStdPackage()
        {
            UnloadStdPackage();
            LoadStdPackage();
        }

        public void LoadDefaultPackages()
        {
            List<PackageInfo> auto = new List<PackageInfo>(_packagesOffline.Count);
            foreach(PackageInfo info in _packagesOffline.Values)
                if(info.isdefault)
                    auto.Add(info);
            // note: don't merge these two loops, because LoadPackage changes the two dictionaries
            foreach(PackageInfo info in auto)
                LoadPackage(info);
        }

        public void UnloadDefaultPackages()
        {
            throw new NotImplementedException();
        }

        public void ReloadDefaultPackages()
        {
            UnloadDefaultPackages();
            LoadDefaultPackages();
        }
    }
}
