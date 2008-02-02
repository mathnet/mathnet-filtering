// Math.NET Neodym, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2008, Christoph Rüegg,  http://christoph.ruegg.name
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
using System.Security;

[assembly: AssemblyTitle("Math.NET Neodym: Signal Prozessing Library")]
[assembly: AssemblyDescription("http://mathnet.opensourcedotnet.info")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Math.NET Project")]
[assembly: AssemblyProduct("Math.NET")]
[assembly: AssemblyCopyright("Copyright © 2008, Math.NET Project")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: PermissionSet(SecurityAction.RequestOptional, Unrestricted = false)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]
[assembly: SecurityPermission(SecurityAction.RequestOptional, SkipVerification = true)] // TODO: remove - temp. fixes NCover issue
[assembly: SecurityPermission(SecurityAction.RequestRefuse, UnmanagedCode = true, Assertion = true, BindingRedirects = true)]
[assembly: FileIOPermission(SecurityAction.RequestRefuse, Unrestricted = true)]
[assembly: EnvironmentPermission(SecurityAction.RequestRefuse, Unrestricted = true)]
[assembly: ReflectionPermission(SecurityAction.RequestRefuse, Unrestricted = true)]
[assembly: RegistryPermission(SecurityAction.RequestRefuse, Unrestricted = true)]
[assembly: System.Net.SocketPermission(SecurityAction.RequestRefuse, Unrestricted = true)]
[assembly: System.Net.WebPermission(SecurityAction.RequestRefuse, Unrestricted = true)]
[assembly: System.Net.DnsPermission(SecurityAction.RequestRefuse, Unrestricted = true)]
[assembly: System.Net.Mail.SmtpPermission(SecurityAction.RequestRefuse, Unrestricted = true)]
[assembly: System.Net.NetworkInformation.NetworkInformationPermission(SecurityAction.RequestRefuse, Unrestricted = true)]

[assembly: AllowPartiallyTrustedCallers]

[assembly: CLSCompliant(true)]

[assembly: Guid("4d30d62e-c708-411f-bc68-8da5621fcff7")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.0.*")] // even = release
[assembly: AssemblyFileVersion("1.0.0.0")]
