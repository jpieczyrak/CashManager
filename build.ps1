cls
cd $PSScriptRoot

$OutDirSetup64 = "..\Builds\Release-64";
$OutDirPortable64 = "..\Builds\Portable-64";
$OutDirPortable32 = "..\Builds\Portable-32";
$Releases = $PSScriptRoot + "\Releases\";


&$Env:msbuild /p:Configuration=Portable /p:Platform="Any CPU" /p:DebugSymbols=false /p:OutputPath=$OutDirPortable32
&$Env:msbuild /p:Configuration=Portable /p:Platform=x64 /p:DebugSymbols=false /p:OutputPath=$OutDirPortable64
&$Env:msbuild /p:Configuration=Release /p:Platform=x64 /p:DebugSymbols=false /p:OutputPath=$OutDirSetup64

$PSScriptRoot + "/CashManager/" + $OutDirPortable32
cd $PSScriptRoot/CashManager/$OutDirPortable32
&$Env:7z a ./../portable-32.7z *.*

$PSScriptRoot + "/CashManager/" + $OutDirPortable64
cd $PSScriptRoot/CashManager/$OutDirPortable64
&$Env:7z a ./../portable-64.7z *.*


$Assembly = [Reflection.Assembly]::Loadfile($PSScriptRoot +"/CashManager/" + $OutDirPortable64 + '/CashManager.exe')
$AssemblyName = $Assembly.GetName()
$Version = $AssemblyName.version
$Version.ToString(3)

Copy-Item ($PSScriptRoot + "/Builds/portable-64.7z") -Destination ($Releases + 'portable-' + $Version.ToString(3) + '.7z');
Copy-Item ($PSScriptRoot + "/Builds/portable-32.7z") -Destination ($Releases + 'portable-' + $Version.ToString(3) + '-32bit.7z');
Copy-Item ($Releases + "Setup.exe") -Destination ($Releases + 'Setup-' + $Version.ToString(3) + '.exe');