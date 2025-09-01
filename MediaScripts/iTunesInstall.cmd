cd C:\Users\%USERNAME%\Downloads

if not exist iTunes (md iTunes)

CALL COPY /Y iTunes64Setup.exe iTunes
cd iTunes
7z e iTunes64Setup.exe -r

REM This is the problem component
REM AppleSoftwareUpdate.msi

REM optional
AppleSoftwareUpdate.msi
PAUSE

REM optional
AppleMobileDeviceSupport64.msi

REM optional
Bonjour64.msi

REM the main installer
iTunes64.msi
