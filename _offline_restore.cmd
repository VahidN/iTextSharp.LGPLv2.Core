set NuGetAudit=false
dotnet restore --ignore-failed-sources -v diag --source %USERPROFILE%\.nuget\packages

pause