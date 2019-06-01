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

let filteringRelease = release "filtering" "Math.NET Filtering" "RELEASENOTES.md"
let releases = [filteringRelease ]
traceHeader releases


// FILTERING PACKAGES

let filteringZipPackage = zipPackage "MathNet.Filtering" "Math.NET Filtering" filteringRelease
let filteringNuGetPackage = nugetPackage "MathNet.Filtering" filteringRelease
let filteringKalmanNuGetPackage = nugetPackage "MathNet.Filtering.Kalman" filteringRelease
let filteringProject = project "MathNet.Filtering" "src/Filtering/Filtering.csproj" [filteringNuGetPackage]
let filteringKalmanProject = project "MathNet.Filtering.Kalman" "src/Kalman/Kalman.csproj" [filteringKalmanNuGetPackage]
let filteringSolution = solution "Filtering" "MathNet.Filtering.sln" [filteringProject; filteringKalmanProject] [filteringZipPackage]

let filteringStrongNameZipPackage = zipPackage "MathNet.Filtering.Signed" "Math.NET Filtering" filteringRelease
let filteringStrongNameNuGetPackage = nugetPackage "MathNet.Filtering.Signed" filteringRelease
let filteringKalmanStrongNameNuGetPackage = nugetPackage "MathNet.Filtering.Kalman.Signed" filteringRelease
let filteringStrongNameProject = project "MathNet.Filtering" "src/Filtering/Filtering.Signed.csproj" [filteringStrongNameNuGetPackage]
let filteringKalmanStrongNameProject = project "MathNet.Filtering.Kalman" "src/Kalman/Kalman.Signed.csproj" [filteringKalmanStrongNameNuGetPackage]
let filteringStrongNameSolution = solution "Filtering.Signed" "MathNet.Filtering.Signed.sln" [filteringStrongNameProject; filteringKalmanStrongNameProject] [filteringStrongNameZipPackage]


// ALL

let allSolutions = [filteringSolution; filteringStrongNameSolution]
let allProjects = allSolutions |> List.collect (fun s -> s.Projects) |> List.distinct


// --------------------------------------------------------------------------------------
// PREPARE
// --------------------------------------------------------------------------------------

Target "Start" DoNothing

Target "Clean" (fun _ ->
    DeleteDirs (!! "src/**/obj/" ++ "src/**/bin/" )
    CleanDirs [ "out/api"; "out/docs" ]
    allSolutions |> List.iter (fun solution -> CleanDirs [ solution.OutputZipDir; solution.OutputNuGetDir; solution.OutputLibDir; solution.OutputLibStrongNameDir ])
    allSolutions |> List.iter clean)

Target "ApplyVersion" (fun _ ->
    allProjects |> List.iter patchVersionInProjectFile
    patchVersionInAssemblyInfo "src/Filtering.Tests" filteringRelease
    patchVersionInAssemblyInfo "src/Kalman.Tests" filteringRelease)

Target "Restore" (fun _ -> allSolutions |> List.iter restore)
"Start"
  =?> ("Clean", not (hasBuildParam "incremental"))
  ==> "Restore"

Target "Prepare" DoNothing
"Start"
  =?> ("Clean", not (hasBuildParam "incremental"))
  ==> "ApplyVersion"
  ==> "Prepare"



// --------------------------------------------------------------------------------------
// BUILD, SIGN, COLLECT
// --------------------------------------------------------------------------------------

let fingerprint = "490408de3618bed0a28e68dc5face46e5a3a97dd"
let timeserver = "http://time.certum.pl/"

Target "Build" (fun _ ->

    // Strong Name Build (with strong name, without certificate signature)
    if hasBuildParam "strongname" then
        CleanDirs (!! "src/**/obj/" ++ "src/**/bin/" )
        restoreSN filteringStrongNameSolution
        buildSN filteringStrongNameSolution
        if isWindows && hasBuildParam "sign" then sign fingerprint timeserver filteringStrongNameSolution
        collectBinariesSN filteringStrongNameSolution
        zip filteringStrongNameZipPackage filteringStrongNameSolution.OutputZipDir filteringStrongNameSolution.OutputLibStrongNameDir (fun f -> f.Contains("MathNet.Filtering.") || f.Contains("MathNet.Numerics."))
        if isWindows then
            packSN filteringStrongNameSolution
            collectNuGetPackages filteringStrongNameSolution

    // Normal Build (without strong name, with certificate signature)
    CleanDirs (!! "src/**/obj/" ++ "src/**/bin/" )
    restore filteringSolution
    build filteringSolution
    if isWindows && hasBuildParam "sign" then sign fingerprint timeserver filteringSolution
    collectBinaries filteringSolution
    zip filteringZipPackage filteringSolution.OutputZipDir filteringSolution.OutputLibDir (fun f -> f.Contains("MathNet.Filtering.") || f.Contains("MathNet.Numerics."))
    if isWindows then
        pack filteringSolution
        collectNuGetPackages filteringSolution

    // NuGet Sign (all or nothing)
    if isWindows && hasBuildParam "sign" then signNuGet fingerprint timeserver [filteringSolution; filteringStrongNameSolution]

    )
"Prepare" ==> "Build"


// --------------------------------------------------------------------------------------
// TEST
// --------------------------------------------------------------------------------------

let testFiltering framework = test "src/Filtering.Tests" "Filtering.Tests.csproj" framework
Target "TestFiltering" DoNothing
Target "TestFilteringCore2.2" (fun _ -> testFiltering "netcoreapp2.2")
Target "TestFilteringNET40" (fun _ -> testFiltering "net40")
Target "TestFilteringNET45" (fun _ -> testFiltering "net45")
Target "TestFilteringNET461" (fun _ -> testFiltering "net461")
Target "TestFilteringNET47"  (fun _ -> testFiltering "net47")
"Build" ==> "TestFilteringCore2.2" ==> "TestFiltering"
"Build" =?> ("TestFilteringNET40", isWindows)
"Build" =?> ("TestFilteringNET45", isWindows)
"Build" =?> ("TestFilteringNET461", isWindows) ==> "TestFiltering"
"Build" =?> ("TestFilteringNET47", isWindows)
let testKalman framework = test "src/Kalman.Tests" "Kalman.Tests.csproj" framework
Target "TestKalman" DoNothing
Target "TestKalmanCore2.2" (fun _ -> testKalman "netcoreapp2.2")
Target "TestKalmanNET45" (fun _ -> testKalman "net45")
Target "TestKalmanNET461" (fun _ -> testKalman "net461")
Target "TestKalmanNET47" (fun _ -> testKalman "net47")
"Build" ==> "TestKalmanCore2.2" ==> "TestKalman"
"Build" =?> ("TestKalmanNET45", isWindows)
"Build" =?> ("TestKalmanNET461", isWindows) ==> "TestKalman"
"Build" =?> ("TestKalmanNET47", isWindows)
Target "Test" DoNothing
"TestFiltering" ==> "Test"
"TestKalman" ==> "Test"


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
    !! "src/Filtering/bin/Release/net40/MathNet.Filtering.dll" ++ "src/Kalman/bin/Release/net40/MathNet.Filtering.Kalman.dll"
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

Target "PublishDocs" (fun _ -> publishDocs filteringRelease)
Target "PublishApi" (fun _ -> publishApi filteringRelease)

Target "PublishArchive" (fun _ -> publishArchives [filteringSolution; filteringStrongNameSolution])

Target "PublishNuGet" (fun _ -> publishNuGet [filteringSolution; filteringStrongNameSolution])

Target "Publish" DoNothing
Dependencies "Publish" [ "PublishTag"; "PublishDocs"; "PublishApi"; "PublishArchive"; "PublishNuGet" ]


// --------------------------------------------------------------------------------------
// Default Targets
// --------------------------------------------------------------------------------------

Target "All" DoNothing
Dependencies "All" [ "Build"; "Docs"; "Api"; "Test" ]

RunTargetOrDefault "Test"
