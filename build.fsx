//  __  __       _   _       _   _ ______ _______
// |  \/  |     | | | |     | \ | |  ____|__   __|
// | \  / | __ _| |_| |__   |  \| | |__     | |
// | |\/| |/ _` | __| '_ \  | . ` |  __|    | |
// | |  | | (_| | |_| | | |_| |\  | |____   | |
// |_|  |_|\__,_|\__|_| |_(_)_| \_|______|  |_|
//
// Math.NET Filtering - https://filtering.mathdotnet.com
// Copyright (c) Math.NET - Open Source MIT/X11 License
//
// FAKE build script, see http://fsharp.github.io/FAKE
//

// --------------------------------------------------------------------------------------
// PRELUDE
// --------------------------------------------------------------------------------------

#I "packages/build/FAKE/tools"
#r "packages/build/FAKE/tools/FakeLib.dll"

open Fake
open Fake.DocuHelper
open System
open System.IO

#load "build/build-framework.fsx"
open BuildFramework


// --------------------------------------------------------------------------------------
// PROJECT INFO
// --------------------------------------------------------------------------------------

// VERSION OVERVIEW

let filteringRelease = release "Math.NET Filtering" "RELEASENOTES.md"
let releases = [filteringRelease ]
traceHeader releases


// CORE PACKAGES

let summary = "Math.NET Filtering, providing methods and algorithms for signal processing and filtering in science, engineering and every day use."
let description = "Math.NET Filtering with finite and infinite impulse response filter design and application, median filtering and other signal processing methods and algorithms. MIT license. "
let descriptionKalman = "Math.NET Filtering: separate package with Kalman filter only. Cannot currently be included into the main package because of licensing restrictions. LGPL license. "
let support = "Supports .Net 4.0 and Mono on Windows, Linux and Mac."
let tags = "math filter signal FIR IIR median kalman design"

let libnet35 = "lib/net35"
let libnet40 = "lib/net40"
let libnet45 = "lib/net45"
let libpcl47 = "lib/portable-net45+sl5+netcore45+MonoAndroid1+MonoTouch1"
let libpcl344 = "lib/portable-net45+sl5+netcore45+wpa81+wp8+MonoAndroid1+MonoTouch1"

let filteringPack =
    { Id = "MathNet.Filtering"
      Release = filteringRelease
      Title = "Math.NET Filtering"
      Summary = summary
      Description = description + support
      Tags = tags
      Authors = [ "Christoph Ruegg" ]
      FsLoader = false
      Dependencies =
        [ { FrameworkVersion=""
            Dependencies=[ "MathNet.Numerics", GetPackageVersion "packages" "MathNet.Numerics" ] } ]
      Files =
        [ @"..\..\out\lib\Net35\MathNet.Filtering.*", Some libnet35, Some @"**\MathNet.Filtering.Kalman.*";
          @"..\..\out\lib\Net40\MathNet.Filtering.*", Some libnet40, Some @"**\MathNet.Filtering.Kalman.*";
          @"..\..\out\lib\Profile259\MathNet.Filtering.*", Some libpcl259, Some @"**\MathNet.Filtering.Kalman.*";
          @"..\..\src\Filtering\**\*.cs", Some "src/Common", None ] }

let kalmanPack =
    { Id = "MathNet.Filtering.Kalman"
      Release = filteringRelease
      Title = "Math.NET Filtering - Kalman Filter"
      Summary = summary
      Description = descriptionKalman + support
      Tags = tags
      Authors = [ "Christoph Ruegg" ]
      FsLoader = false
      Dependencies =
        [ { FrameworkVersion=""
            Dependencies=[ "MathNet.Numerics", GetPackageVersion "packages" "MathNet.Numerics" ] } ]
      Files =
        [ @"..\..\out\lib\Net35\MathNet.Filtering.Kalman.*", Some libnet35, None;
          @"..\..\out\lib\Net40\MathNet.Filtering.Kalman.*", Some libnet40, None;
          @"..\..\out\lib\Profile259\MathNet.Filtering.Kalman.*", Some libpcl259, None;
          @"..\..\src\Kalman\**\*.cs", Some "src/Common", None ] }

let coreBundle =
    { Id = filteringPack.Id
      Release = filteringRelease
      Title = filteringPack.Title
      Packages = [ filteringPack; kalmanPack ] }


// --------------------------------------------------------------------------------------
// PREPARE
// --------------------------------------------------------------------------------------

Target "Start" DoNothing

Target "Clean" (fun _ ->
    CleanDirs [ "obj" ]
    CleanDirs [ "out/api"; "out/docs"; "out/packages" ]
    CleanDirs [ "out/lib/Net35"; "out/lib/Net40"; "out/lib/Profile259" ]
    CleanDirs [ "out/test/Net35"; "out/test/Net40"; "out/test/Profile259" ])

Target "ApplyVersion" (fun _ ->
    patchVersionInAssemblyInfo "src/Filtering" filteringRelease
    patchVersionInAssemblyInfo "src/FilteringUnitTests" filteringRelease
    patchVersionInAssemblyInfo "src/Kalman" filteringRelease
    patchVersionInAssemblyInfo "src/KalmanUnitTests" filteringRelease)

Target "Prepare" DoNothing
"Start"
  =?> ("Clean", not (hasBuildParam "incremental"))
  ==> "ApplyVersion"
  ==> "Prepare"


// --------------------------------------------------------------------------------------
// BUILD
// --------------------------------------------------------------------------------------

Target "BuildMain" (fun _ -> build !! "MathNet.Filtering.sln")
Target "BuildNet35" (fun _ -> build !! "MathNet.Filtering.Net35Only.sln")
Target "BuildAll" (fun _ -> build !! "MathNet.Filtering.All.sln")

Target "Build" DoNothing
"Prepare"
  =?> ("BuildNet35", hasBuildParam "net35")
  =?> ("BuildAll", hasBuildParam "all" || hasBuildParam "release")
  =?> ("BuildMain", not (hasBuildParam "all" || hasBuildParam "release" || hasBuildParam "net35"))
  ==> "Build"


// --------------------------------------------------------------------------------------
// TEST
// --------------------------------------------------------------------------------------

Target "Test" (fun _ -> test !! "out/test/**/*UnitTests*.dll")
"Build" ==> "Test"


// --------------------------------------------------------------------------------------
// PACKAGES
// --------------------------------------------------------------------------------------

Target "Pack" DoNothing

// ZIP

Target "Zip" (fun _ ->
    CleanDir "out/packages/Zip"
    coreBundle |> zip "out/packages/Zip" "out/lib" (fun f -> f.Contains("MathNet.Filtering.") || f.Contains("MathNet.Numerics.")))
"Build" ==> "Zip" ==> "Pack"

// NUGET

Target "NuGet" (fun _ ->
    CleanDir "out/packages/NuGet"
    if hasBuildParam "all" || hasBuildParam "release" then
        nugetPack coreBundle "out/packages/NuGet")
"Build" ==> "NuGet" ==> "Pack"


// --------------------------------------------------------------------------------------
// Documentation
// --------------------------------------------------------------------------------------

// DOCS

Target "CleanDocs" (fun _ -> CleanDirs ["out/docs"])

let extraDocs =
    [ "LICENSE.md", "License.md"
      "CONTRIBUTING.md", "Contributing.md"
      "CONTRIBUTORS.md", "Contributors.md" ]

Target "Docs" (fun _ ->
    provideDocExtraFiles extraDocs releases
    generateDocs true false)
Target "DocsDev" (fun _ ->
    provideDocExtraFiles extraDocs releases
    generateDocs true true)
Target "DocsWatch" (fun _ ->
    provideDocExtraFiles extraDocs releases
    use watcher = new FileSystemWatcher(DirectoryInfo("docs/content").FullName, "*.*")
    watcher.EnableRaisingEvents <- true
    watcher.Changed.Add(fun e -> generateDocs false true)
    watcher.Created.Add(fun e -> generateDocs false true)
    watcher.Renamed.Add(fun e -> generateDocs false true)
    watcher.Deleted.Add(fun e -> generateDocs false true)
    traceImportant "Waiting for docs edits. Press any key to stop."
    System.Console.ReadKey() |> ignore
    watcher.EnableRaisingEvents <- false
    watcher.Dispose())

"Build" ==> "CleanDocs" ==> "Docs"

"Start"
  =?> ("CleanDocs", not (hasBuildParam "incremental"))
  ==> "DocsDev"
  ==> "DocsWatch"


// API REFERENCE

Target "CleanApi" (fun _ -> CleanDirs ["out/api"])

Target "Api" (fun _ ->
    !! "out/lib/Net40/MathNet.Filtering.dll" ++ "out/lib/Net40/MathNet.Filtering.Kalman.dll"
    |> Docu (fun p ->
        { p with
            ToolPath = "tools/docu/docu.exe"
            TemplatesPath = "tools/docu/templates/"
            TimeOut = TimeSpan.FromMinutes 10.
            OutputPath = "out/api/" }))

"Build" ==> "CleanApi" ==> "Api"


// --------------------------------------------------------------------------------------
// Publishing
// Requires permissions; intended only for maintainers
// --------------------------------------------------------------------------------------

Target "PublishTag" (fun _ -> publishReleaseTag "Math.NET Filtering" "" filteringRelease)

Target "PublishMirrors" (fun _ -> publishMirrors ())
Target "PublishDocs" (fun _ -> publishDocs filteringRelease)
Target "PublishApi" (fun _ -> publishApi filteringRelease)

Target "PublishArchive" (fun _ -> publishArchive "out/packages/Zip" "out/packages/NuGet" [coreBundle])

Target "PublishNuGet" (fun _ -> !! "out/packages/NuGet/*.nupkg" -- "out/packages/NuGet/*.symbols.nupkg" |> publishNuGet)

Target "Publish" DoNothing
Dependencies "Publish" [ "PublishTag"; "PublishDocs"; "PublishApi"; "PublishArchive"; "PublishNuGet" ]


// --------------------------------------------------------------------------------------
// Default Targets
// --------------------------------------------------------------------------------------

Target "All" DoNothing
Dependencies "All" [ "Pack"; "Docs"; "Api"; "Test" ]

RunTargetOrDefault "Test"
