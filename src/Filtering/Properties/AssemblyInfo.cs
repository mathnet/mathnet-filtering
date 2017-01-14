using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

#if !PORTABLE
using System.Runtime.InteropServices;
#endif

[assembly: AssemblyDescription("Math.NET Filtering")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Math.NET Project")]
[assembly: AssemblyProduct("Math.NET Filtering")]
[assembly: AssemblyCopyright("Copyright (c) Math.NET Project")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en")]

[assembly: AssemblyVersion("0.3.0.0")]
[assembly: AssemblyFileVersion("0.3.0.0")]
[assembly: AssemblyInformationalVersion("0.3.0")]

#if PORTABLE

[assembly: AssemblyTitle("Math.NET Filtering - Portable Edition")]
[assembly: InternalsVisibleTo("MathNet.Filtering.UnitTests")]
[assembly: InternalsVisibleTo("MathNet.Filtering.UnitTests259")]

#elif NET35

[assembly: AssemblyTitle("Math.NET Filtering - .Net 3.5 Edition")]
[assembly: InternalsVisibleTo("MathNet.Filtering.UnitTests")]
[assembly: InternalsVisibleTo("MathNet.Filtering.UnitTestsNet35")]

#else

[assembly: AssemblyTitle("Math.NET Filtering")]
[assembly: ComVisible(false)]
[assembly: Guid("4d30d62e-c708-411f-bc68-8da5621fcff7")]
[assembly: InternalsVisibleTo("MathNet.Filtering.UnitTests")]

#endif
