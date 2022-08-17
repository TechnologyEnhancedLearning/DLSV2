@echo off
IF EXIST WinSCP.log (
    del WinSCP.log
)

rem deploy to filesystem
SET PUBLISH_PROFILE_PATH=
dotnet build DigitalLearningSolutions.Web/DigitalLearningSolutions.Web.csproj -c Release /p:DeployOnBuild=true /p:PublishProfile=DigitalLearningSolutions.Web/Properties/PublishProfiles/%2.pubxml
if %ERRORLEVEL% neq 0 goto builderror

rem ftp upload
"C:\Program Files (x86)\WinSCP\WinSCP.exe" /log="WinSCP.log" /ini=nul /script="DeployToFtpServer.txt" /parameter %1 %3 %4
if %ERRORLEVEL% neq 0 goto ftperror

echo Deployment succeeded
exit /b 0

:builderror
echo Publish to folder failed, please run 'yarn build' in the DigitalLearningSolutions.Web folder before trying again
exit /b 1

:ftperror
echo Ftp transfer failed
FOR /F "tokens=* delims=" %%x in (WinSCP.log) DO (
    rem do not print a line containing the word "Command-line" as that line will include the ftp password
    (echo %%x | findstr /i /c:"Command-line" >nul) || (echo %%x)
)
exit /b 1
