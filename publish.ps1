$outDir = '.\output'

if (Test-Path $outDir) {
	rm -Force -Recurse $outDir
}
mkdir $outDir | Out-Null

dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:DebugType=embedded -r win-x64 .\src\FirebirdMonitorTool.ConsoleProfiler -o $outDir\win
dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:DebugType=embedded -r linux-x64 .\src\FirebirdMonitorTool.ConsoleProfiler -o $outDir\linux

Compress-Archive -Path $outDir\win\* -DestinationPath $outDir\FirebirdMonitorTool-win.zip -CompressionLevel Optimal
Compress-Archive -Path $outDir\linux\* -DestinationPath $outDir\FirebirdMonitorTool-linux.zip -CompressionLevel Optimal

rm -Force -Recurse $outDir\win
rm -Force -Recurse $outDir\linux
