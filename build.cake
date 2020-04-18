#addin nuget:?package=SharpZipLib&Version=1.2.0
#addin nuget:?package=Cake.Compression&Version=0.2.4

const string buildTarget = "build";
const string buildDockerTarget = "build_docker";
const string unitTestTarget = "unit_test";
const string integrationTestTarget = "integration_test";
const string testTarget = "test";
const string publishDockerTarget = "publish_docker";
const string publishWindowsTarget = "publish_windows";
const string publishUnixTarget = "publish_unix";
const string publishAllTarget = "publish";
const string packageWindowsTarget = "package_windows";
const string packageUnixTarget = "package_unix";
const string packageTarget = "package";

string target = Argument( "target", buildTarget );
bool skipPublish = Argument<bool>( "skip_publish", false );
bool deleteIntegrationTestDir = Argument<bool>( "delete_test_repos", true );

FilePath sln = File( "./svn2gitnetx.sln" );
DirectoryPath packageDir = Directory( "./dist" );

const string coreAppVers = "netcoreapp3.1";

List<string> unixRunTimes = new List<string>
{
    "ubuntu.16.04-x64",
    "centos.7-x64",
    "debian.8-x64",
    "fedora.24-x64",
    "rhel.7-x64",
    "osx.10.12-x64"
};

// ---------------- Build ----------------

Task( buildTarget )
.Does(
    () =>
    {
        DotNetCoreBuild( sln.ToString() );
    }
).Description( "Builds Svn2GitNet and Tests" );

// ---------------- Unit Test ----------------

Task( unitTestTarget )
.Does(
    () =>
    {
        DotNetCoreTestSettings settings = new DotNetCoreTestSettings 
        {
            // Already compiled due to dependent target.
            NoBuild = true,
            NoRestore = true
        };
        DotNetCoreTest( "./tests/unittests/svn2gitnetx.unittests.csproj", settings );
    }
).Description( "Runs Unit Tests" )
.IsDependentOn( buildTarget );

// ---------------- Publishing ----------------

private void DoPublish( string runTime )
{
    DotNetCorePublishSettings settings = new DotNetCorePublishSettings
    {
        Configuration = "Release",
        SelfContained = true,
        Runtime = runTime
    };

    DotNetCorePublish( "./src/svn2gitnetx.csproj", settings );
}

Task( publishWindowsTarget )
.Does(
    () =>
    {
        List<string> runTimes = new List<string>
        {
            "win10-x64",
            "win10-x86"
        };

        foreach( string runTime in runTimes )
        {
            DoPublish( runTime );
        }
    }
)
.Description( "Publishes ONLY the Windows targets" )
.IsDependentOn( unitTestTarget );

Task( publishUnixTarget )
.Does(
    () =>
    {
        foreach( string runTime in unixRunTimes )
        {
            DoPublish( runTime );
        }
    }
)
.Description( "Publishes ONLY the Unix targets" )
.IsDependentOn( unitTestTarget );

Task( publishAllTarget )
.Description( "Publishes ALL targets" )
.IsDependentOn( publishWindowsTarget )
.IsDependentOn( publishUnixTarget );

// ---------------- Integration Testing ----------------

var integrationTestTask = Task( integrationTestTarget )
.Does( 
    () =>
    {
        DirectoryPath integrationTestDir = Directory( "integrationtests" );
        EnsureDirectoryExists( integrationTestDir );
        CleanDirectory( integrationTestDir );

        try
        {
            CopyDirectory(
                Directory( "./src/bin/Release" ),
                integrationTestDir
            );

            DotNetCoreTestSettings settings = new DotNetCoreTestSettings 
            {
                // Already compiled due to dependent target.
                NoBuild = true,
                NoRestore = true,
                WorkingDirectory = "./tests/integrationtests"
            };
            DotNetCoreTest( "./svn2gitnetx.integrationtests.csproj", settings );
        }
        finally
        {
            if( deleteIntegrationTestDir )
            {
                DeleteDirectorySettings delSettings = new DeleteDirectorySettings
                {
                    Recursive = true,
                    Force = true
                };
                DeleteDirectory( integrationTestDir, delSettings );
            }
        }
    }
).Description( "Runs Integration Tests" );

Task( testTarget )
.Description( "Runs both unit and integration tests" )
.IsDependentOn( unitTestTarget )
.IsDependentOn( integrationTestTarget );

// ---------------- Packaging ----------------

var packageWindowsTask = Task( packageWindowsTarget )
.Does(
    () =>
    {
        EnsureDirectoryExists( packageDir );
        List<string> platforms = new List<string>
        {
            "x86",
            "x64"
        };

        foreach( string platform in platforms )
        {
            DirectoryPath packageFolder = packageDir.Combine( "win-" + platform );
            FilePath wxsFile = packageFolder.CombineWithFilePath( $"svn2gitnetx-{platform}.wxs" );
            EnsureDirectoryExists( packageFolder );
            CleanDirectory( packageFolder );

            // -------- Heat --------

            Information( "Starting Heat for " + platform );
            HeatSettings heatSettings = new HeatSettings
            {
                ComponentGroupName = "svn2gitnetx", // -cg
                GenerateGuid = true,               // -gg
                SuppressFragments = true,          // -sfrag
                SuppressRegistry = true,           // -sreg
                SuppressVb6Com = true,             // -svb6
                Template = WiXTemplateType.Product,// -template product
                Transform = "svn2gitnetx.xslt",
                ToolPath = @"C:\Program Files (x86)\WiX Toolset v3.11\bin\heat.exe"
            };

            WiXHeat(
                Directory( $"./src/bin/Release/{coreAppVers}/win10-{platform}/publish" ),
                wxsFile,
                WiXHarvestType.Dir,
                heatSettings
            );

            // -------- Candle --------

            Information( "Starting Candle for " + platform );

            CandleSettings candleSettings = new CandleSettings
            {
                WorkingDirectory = packageFolder.ToString(),
                ToolPath = @"C:\Program Files (x86)\WiX Toolset v3.11\bin\candle.exe"
            };

            WiXCandle( wxsFile.ToString(), candleSettings );

            // -------- Light --------

            Information( "Starting Light for " + platform );
            LightSettings lightSettings = new LightSettings
            {
                RawArguments = $"-ext WixUIExtension -cultures:en-us -b .\\src\\bin\\Release\\{coreAppVers}\\win10-{platform}\\publish",
                OutputFile = packageFolder.CombineWithFilePath( $"svn2gitnetx-{platform}.msi" ),
                ToolPath = @"C:\Program Files (x86)\WiX Toolset v3.11\bin\light.exe"
            };

            FilePath wixObjFile = packageFolder.CombineWithFilePath( $"svn2gitnetx-{platform}.wixobj" );
            WiXLight( wixObjFile.ToString(), lightSettings );
        }
    }
)
.Description( "Creates the windows .MSI file(s).  Windows only." )
.WithCriteria( Environment.OSVersion.Platform == PlatformID.Win32NT );

var packageUnixTask = Task( packageUnixTarget )
.Does(
    () =>
    {
        foreach( string platform in unixRunTimes )
        {
            Information( $"Packing {platform}..." );

            DirectoryPath packageFolder = packageDir.Combine( platform );
            FilePath tarFile = packageFolder.CombineWithFilePath( $"svn2gitnetx-{platform}.tar.gz" );
            EnsureDirectoryExists( packageFolder );
            CleanDirectory( packageFolder );

            GZipCompress(
                $"./src/bin/Release/{coreAppVers}/{platform}/publish",
                tarFile,
                6
            );
        }
    }
)
.Description( "Creates the tar-balls for unix platforms" );

if( skipPublish == false )
{
    if( Environment.OSVersion.Platform == PlatformID.Win32NT )
    {
        integrationTestTask.IsDependentOn( publishWindowsTarget );
    }
    else
    {
        integrationTestTask.IsDependentOn( publishUnixTarget );
    }

    packageWindowsTask.IsDependentOn( publishWindowsTarget );
    packageUnixTask.IsDependentOn( publishUnixTarget );
}

Task( packageTarget )
.Description( "Packages both Windows and Unix" )
.IsDependentOn( packageWindowsTarget )
.IsDependentOn( packageUnixTarget );

// ---------------- Docker ----------------

Task( publishDockerTarget )
.Does(
    () =>
    {
        DirectoryPath dockerDist = packageDir.Combine( "docker" );
        EnsureDirectoryExists( dockerDist );
        CleanDirectory( dockerDist );

        DotNetCorePublishSettings settings = new DotNetCorePublishSettings
        {
            Configuration = "Release",
            SelfContained = false,
            Runtime = "linux-x64",
            OutputDirectory = dockerDist
        };

        DotNetCorePublish( "./src/svn2gitnetx.csproj", settings );
    }
).Description( "Builds for making the docker image." );

Task( buildDockerTarget )
.Does(
    () =>
    {
        string arguments = "build -t svn2gitnetx -f ./docker/Dockerfile ./dist/docker";
        ProcessArgumentBuilder argumentsBuilder = ProcessArgumentBuilder.FromString( arguments );
        ProcessSettings settings = new ProcessSettings
        {
            Arguments = argumentsBuilder
        };
        int exitCode = StartProcess( "docker", settings );
        if( exitCode != 0 )
        {
            throw new ApplicationException(
                "Error when running docker.  Got error: " + exitCode
            );
        }
    }
)
.Description( "Builds the docker image" )
.IsDependentOn( publishDockerTarget );

// ---------------- All ----------------

Task( "all" )
.Description( "Does everything" )
.IsDependentOn( testTarget )
.IsDependentOn( publishAllTarget )
.IsDependentOn( packageTarget );

RunTarget( target );