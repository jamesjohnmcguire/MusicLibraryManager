IF "%1"=="msbuild" GOTO msbuild
GOTO default

:msubuild
@ECHO OFF
@ECHO usage: Build ^<target type (BuildAll, Test, Publish)^> ^<Configuration (Debug, Release)^> ^<build type (Rebuild, Build, Clean)^>
@ECHO defaults: Rebuild Release

CD %~dp0

REM Run customized build project
SET target=%1
SET configuration=%2
SET buildType=%3

REM Set defaults, if needed
IF [%1]==[] SET target=BuildAll
IF [%2]==[] SET configuration=Release
IF [%3]==[] SET buildType=Rebuild

@ECHO target: %target%
@ECHO configuration: %configuration%
@ECHO buildType: %buildType%

msbuild MusicLibrary.msbuild.xml /p:BuildType=%buildType%;Configuration=%configuration%;SolutionPath=..\..;Platform=x64 /t:%target% -restore

:default
CD %~dp0
CD ..\..

REM IF "%1"=="release" CALL VersionUpdate BackUpManagerLibrary\BackupManagerLibrary.csproj
REM IF "%1"=="release" CALL VersionUpdate BackUpManager\BackupManager.csproj

CALL msbuild -property:Configuration=Release;IncludeAllContentForSelfExtract=true;Platform="Any CPU";PublishReadyToRun=true;PublishSingleFile=true;PublishTrimmed=True;Runtimeidentifier=win-x64;SelfContained=true -restore -target:publish;rebuild MusicManager

IF "%1"=="release" GOTO release
GOTO end

:release
CD Bin\Release\AnyCPU\win-x64\publish

7z u MusicManager.zip .

hub release create -a MusicManager.zip -m "%2" v%2

:end
