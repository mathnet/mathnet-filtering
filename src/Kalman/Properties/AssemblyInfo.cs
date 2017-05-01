using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

#if !PORTABLE
using System.Runtime.InteropServices;
#endif

[assembly: AssemblyDescription("Math.NET Filtering - Kalman Filter")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Math.NET Project")]
[assembly: AssemblyProduct("Math.NET Filtering")]
[assembly: AssemblyCopyright("Copyright (c) Math.NET Project")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en")]

[assembly: AssemblyVersion("0.4.0.0")]
[assembly: AssemblyFileVersion("0.4.0.0")]
[assembly: AssemblyInformationalVersion("0.4.0")]

#if PORTABLE

[assembly: AssemblyTitle("Math.NET Filtering - Kalman Filter - Portable Edition")]

#elif NET35

[assembly: AssemblyTitle("Math.NET Filtering - Kalman Filter - .Net 3.5 Edition")]

#else

[assembly: AssemblyTitle("Math.NET Filtering - Kalman Filter")]
[assembly: ComVisible(false)]
[assembly: Guid("DDFDCE89-CCC0-4BE1-8B95-B88E38B7903F")]

#endif
