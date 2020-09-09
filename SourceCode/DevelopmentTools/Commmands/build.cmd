@ECHO OFF
@ECHO usage: Build ^<target type (BuildAll, Test, Publish)^> ^<Configuration (Debug, Release)^> ^<build type (Rebuild, Build, Clean)^>
@ECHO defaults: Rebuild Release
@ECHO.
@ECHO You will need to have msbuild.exe on your path.  You can use the "Developer Command Prompt for VSXXXX" or add "%ProgramFiles(x86)%\MSBuild\XX.0\bin" to your path.

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
