CD %~dp0
CD ..\SourceCode\MusicManager\Properties

IF "%1"=="clean" GOTO clean
IF "%1"=="extract" GOTO extract

:clean
sed -i "s|[A-Za-z0-9_.]* C:|clean C:|g" launchSettings.json
GOTO finish

:extract
sed -i "s|[A-Za-z0-9_.]* C:|extract-tags C:|g" launchSettings.json

:finish
