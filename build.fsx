// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake
open Fake.Testing

MSBuildDefaults.Verbosity = Some(Quiet)

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"
let testDir = "./test/"
// define test dlls
let testDlls = !! (testDir + "/Sample*.dll")

// Filesets
let appReferences  =
    !! "/**/Sample*.*proj" -- "/**/*Tests.*proj"
let appTestsReferences  =
    !! "/**/*Tests.*proj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir; testDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
        -- "*.zip"
        |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

Target "BuildTest" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug testDir "Build" appTestsReferences
        |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    testDlls
        |> xUnit2 (fun p -> 
            {p with 
                ShadowCopy = false;
                HtmlOutputPath = Some (testDir @@ "html") })
)

"Clean"
  ==> "BuildTest"
  ==> "Test"

// start build
RunTargetOrDefault "Test"
