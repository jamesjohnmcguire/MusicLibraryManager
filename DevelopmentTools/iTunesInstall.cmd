CD C:\Users\%USERNAME%\Downloads

IF NOT EXIST iTunes (MD iTunes)

COPY /Y iTunes64Setup.exe iTunes
CD iTunes
7z e iTunes64Setup.exe -r

REM This is the problem component
REM AppleSoftwareUpdate.msi

REM optional
AppleSoftwareUpdate.msi

REM optional
AppleMobileDeviceSupport64.msi

REM optional
Bonjour64.msi

REM the main installer
iTunes64.msi
