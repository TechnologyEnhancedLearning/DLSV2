rem deploy to filesystem
dotnet publish DigitalLearningSolutions.Web/DigitalLearningSolutions.Web.csproj /p:PublishProfile=DigitalLearningSolutions.Web/Properties/PublishProfiles/PublishToFolderForUAT.pubxml

rem ftp upload
%1 /log="WinSCP.log" /ini=nul /script="DeployToUAT.txt" /parameter %2
if %ERRORLEVEL% neq 0 goto error 

echo Upload succeeded
exit /b 0

:error
echo Upload failed
exit /b 1