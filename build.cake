#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool nuget:?package=GitVersion.CommandLine
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var buildDirs = GetDirectories("./src/**/bin/*") + GetDirectories("./src/**/obj/*");
var builtAssembliesToPackage = new [] {
    "./src/TfsNotificationRelay/bin/" + configuration + "/DevCore.TfsNotificationRelay.dll",
    "./src/TfsNotificationRelay.Slack/bin/" + configuration + "/DevCore.TfsNotificationRelay.Slack.dll",
    "./src/TfsNotificationRelay.HipChat/bin/" + configuration + "/DevCore.TfsNotificationRelay.HipChat.dll",
    "./src/TfsNotificationRelay.MsTeams/bin/" + configuration + "/DevCore.TfsNotificationRelay.MsTeams.dll",
    "./src/TfsNotificationRelay.Smtp/bin/" + configuration + "/DevCore.TfsNotificationRelay.Smtp.dll",
};
var extraFilesToPackage = new [] {
    "./README.md",
    "./COPYING",
    "./src/TfsNotificationRelay/bin/" + configuration + "/DevCore.TfsNotificationRelay.dll.config",
    "./src/TfsNotificationRelay/bin/" + configuration + "/Newtonsoft.Json.dll",
    "./src/TfsNotificationRelay/bin/" + configuration + "/HtmlAgilityPack.dll"
};

var rnd = new Random();

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories(buildDirs);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("TfsNotificationRelay.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("TfsNotificationRelay.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("TfsNotificationRelay.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    MSTest("./src/**/bin/" + configuration + "/*.Tests.dll", 
        new MSTestSettings()
        {
            NoIsolation = false 
        });
});

Task("Sign")
    .IsDependentOn("Build")
    .Does(() =>
{
    Sign(builtAssembliesToPackage, new SignToolSignSettings {
        TimeStampUri = new Uri("http://timestamp.digicert.com"),
        DigestAlgorithm = SignToolDigestAlgorithm.Sha256,
        CertSubjectName = "Kristian Adrup"
    });
});

Task("Zip")
    .IsDependentOn("Build")
    .Does(() =>
{
    var tempdir = "./artifacts/tmp" + rnd.Next(1000);
    var gitVersion =  GitVersion();
    var packageFileName = string.Format("TfsNotificationRelay-{0}.zip", gitVersion.SemVer);
    CreateDirectory(tempdir);
    CopyFiles(builtAssembliesToPackage, tempdir);
    CopyFiles(extraFilesToPackage, tempdir);
    Zip(tempdir, "./artifacts/" + packageFileName);
    DeleteDirectory(tempdir, true);
});

Task("Package")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Sign")
    .IsDependentOn("Zip");

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
