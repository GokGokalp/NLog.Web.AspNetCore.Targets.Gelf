if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

dotnet restore

Invoke-MSBuild -Path "NLog.Web.AspNetCore.Targets.Gelf.sln" -MsBuildParameters "/target:Clean;Build /property:Configuration=Release"

dotnet test .\NLog.Web.AspNetCore.Targets.Gelf.Tests\NLog.Web.AspNetCore.Targets.Gelf.Tests.csproj -c Release

dotnet pack .\NLog.Web.AspNetCore.Targets.Gelf\NLog.Web.AspNetCore.Targets.Gelf.csproj -c Release -o .\artifacts /p:BuildNumber=$env:APPVEYOR_BUILD_VERSION