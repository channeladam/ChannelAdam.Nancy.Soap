SET msbuild="C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"

%msbuild% ..\src\ChannelAdam.Nancy.Soap\ChannelAdam.Nancy.Soap.csproj /t:Rebuild /p:Configuration=Release;TargetFrameworkVersion=v4.0;OutDir=bin\Release\net403

..\tools\nuget\nuget.exe pack ..\src\ChannelAdam.Nancy.Soap\ChannelAdam.Nancy.Soap.nuspec -Symbols

pause
