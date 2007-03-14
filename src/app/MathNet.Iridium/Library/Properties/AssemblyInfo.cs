// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2004-2007, Christoph Rüegg,  http://christoph.ruegg.name
//                          Joannes Vermorel, http://www.vermorel.com
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published 
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

[assembly: AssemblyTitle("Math.NET Iridium: Scientific Numerical Library")]
[assembly: AssemblyDescription("http://mathnet.opensourcedotnet.info/")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Math.NET Project")]
[assembly: AssemblyProduct("Math.NET")]
[assembly: AssemblyCopyright("Copyright © 2006, Math.NET Project")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

// Refuse anything except SecurityPermission.Execution and identity permissions:
// TODO: Should be "Execution"
[assembly: PermissionSet(SecurityAction.RequestOptional, Name="FullTrust")] 

[assembly: Guid("1386fccc-7816-4311-a04b-25227138e8b3")]

[assembly: AssemblyVersion("2007.3.8.*")] // even = release
[assembly: AssemblyFileVersion("2007.3.8.0")]
