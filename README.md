Math.NET Filtering
==================

Math.NET Filtering is a **digital signal processing toolkit**, offering an infrastructure for digital filter design, applying those filters to data streams using data converters, as well as digital signal generators..

Supports Mono and .NET 4.0 on Linux, Mac and Windows.

Math.NET Filtering is covered under the terms of the [MIT/X11](LICENSE.md) license. You may therefore link to it and use it in both opensource and proprietary software projects.

**[Release Notes & Changes](RELEASENOTES.md)**

Installation Instructions
-------------------------

The recommended way to get Math.NET Filtering is to use NuGet. The following packages are provided and maintained in the public [NuGet Gallery](https://nuget.org/profiles/mathnet/):

- **MathNet.Filtering** - core package (MIT license)
- **MathNet.Filtering.Kalman** - Kalman filter (LGPL license - until we can relicense)

Supported Platforms:

- .Net 4.0 and Mono: Windows, Linux and Mac.

Building Math.NET Filtering
---------------------------

Windows (.Net): [![AppVeyor build status](https://ci.appveyor.com/api/projects/status/swi94lke268vaoq8/branch/master)](https://ci.appveyor.com/project/cdrnet/mathnet-filtering)  
Linux (Mono): [![Travis Build Status](https://travis-ci.org/mathnet/mathnet-filtering.svg?branch=master)](https://travis-ci.org/mathnet/mathnet-filtering)

If you do not want to use the official binaries, or if you like to modify, debug or contribute, you can compile Math.NET Filtering locally either using Visual Studio or manually with the build scripts.

* The Visual Studio solutions should build out of the box, without any preparation steps or package restores.
* Instead of a compatible IDE you can also build the solutions with `msbuild`, or on Mono with `xbuild`.
* The full build including unit tests, docs, NuGet and Zip packages is using [FAKE](http://fsharp.github.io/FAKE/).

### How to build with MSBuild/XBuild

    restore.cmd (or restore.sh)
    msbuild MathNet.Filtering.sln            # only build for .Net 4 (main solution)
    msbuild MathNet.Filtering.Net35Only.sln  # only build for .Net 3.5
    msbuild MathNet.Filtering.All.sln        # full build with .Net 4, 3.5 and PCL profiles
    xbuild MathNet.Filtering.sln             # build with Mono, e.g. on Linux or Mac

### How to build with FAKE

    build.cmd    # normal build (.Net 4.0), run unit tests
    ./build.sh   # normal build (.Net 4.0), run unit tests - on Linux or Mac
    ./buildn.sh  # normal build (.Net 4.0), run unit tests - bash on Windows (.Net instead of mono)

    build.cmd Build              # normal build (.Net 4.0)
    build.cmd Build incremental  # normal build, incremental (.Net 4.0)
    build.cmd Build all          # full build (.Net 4.0, 3.5, PCL)
    build.cmd Build net35        # compatibility build (.Net 3.5)

    build.cmd Test        # normal build (.Net 4.0), run unit tests
    build.cmd Test quick  # normal build (.Net 4.0), run unit tests except long running ones
    build.cmd Test all    # full build (.Net 4.0, 3.5, PCL), run all unit tests
    build.cmd Test net35  # compatibility build (.Net 3.5), run unit testss

    build.cmd Clean  # cleanup build artifacts
    build.cmd Docs   # generate documentation
    build.cmd Api    # generate api reference

    build.cmd NuGet all     # generate normal NuGet packages (.Net 4.0, 3.5, PCL)
    build.cmd NuGet signed  # generate signed/strong named NuGet packages (.Net 4.0)

    build.cmd All          # build, test, docs, api reference (.Net 4.0)
    build.cmd All release  # release build

FAKE itself is not included in the repository but it will download and bootstrap itself automatically when build.cmd is run the first time. Note that this step is *not* required when using Visual Studio or `msbuild` directly.

Quick Links
-----------

* [**Project Website**](https://filtering.mathdotnet.com)
* [Source Code](https://github.com/mathnet/mathnet-filtering)
* [Documentation](https://filtering.mathdotnet.com/docs/)
* [API Reference](https://filtering.mathdotnet.com/api/)
* [Work Items and Bug Tracker](https://github.com/mathnet/mathnet-filtering/issues)

Math.NET on other sites:

* [Twitter @MathDotNet](https://twitter.com/MathDotNet)
* [Google+](https://plus.google.com/112484567926928665204)
* [Ohloh](https://www.ohloh.net/p/mathnet)
* [Stack Overflow](https://stackoverflow.com/questions/tagged/mathdotnet)
