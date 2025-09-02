SET utils=%USERPROFILE%\Data\Commands
SET source=%USERPROFILE%\Data\Clients\DigitalZenWorks\MusicLibraryManager\SourceCode\MusicManager\Bin\Release\publish
xcopy /D /E /H /I /R /S /Y %source% %utils%
