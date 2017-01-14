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

[assembly: AssemblyVersion("0.2.0.0")]
[assembly: AssemblyFileVersion("0.2.0.0")]
[assembly: AssemblyInformationalVersion("0.2.0-alpha")]

#if PORTABLE

[assembly: AssemblyTitle("Math.NET Filtering - Kalman Filter - Portable Edition")]
[assembly: InternalsVisibleTo("MathNet.Filtering.UnitTests")]
[assembly: InternalsVisibleTo("MathNet.Filtering.UnitTests259")]

#elif NET35

[assembly: AssemblyTitle("Math.NET Filtering - Kalman Filter - .Net 3.5 Edition")]
[assembly: InternalsVisibleTo("MathNet.Filtering.UnitTests")]
[assembly: InternalsVisibleTo("MathNet.Filtering.UnitTestsNet35")]

#else

[assembly: AssemblyTitle("Math.NET Filtering")]
[assembly: ComVisible(false)]
[assembly: Guid("DDFDCE89-CCC0-4BE1-8B95-B88E38B7903F")]
[assembly: InternalsVisibleTo("MathNet.Filtering.UnitTests")]

#endif
