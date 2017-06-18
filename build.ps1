if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

EnsurePsbuildInstalled

exec { & dotnet restore }

Invoke-MSBuild

exec { & dotnet test .\NLog.Web.AspNetCore.Targets.Gelf.Tests\NLog.Web.AspNetCore.Targets.Gelf.Tests.csproj -c Release }

exec { & dotnet pack .\NLog.Web.AspNetCore.Targets.Gelf\NLog.Web.AspNetCore.Targets.Gelf.csproj -c Release -o .\artifacts /p:BuildNumber=$env:APPVEYOR_BUILD_VERSION }