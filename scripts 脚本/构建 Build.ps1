[CmdletBinding()]
param([string]$FFmpegPath, [string]$OutDir)
$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
if($env:OS -ne 'Windows_NT'){throw 'Build requires Windows / 需要 Windows'}
if(!$OutDir){$OutDir=Join-Path $root '.build 本地构建\bin'}
$OutDir=[IO.Path]::GetFullPath($OutDir)
$compiler=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if(!(Test-Path -LiteralPath $compiler)){throw '.NET Framework 4.x C# compiler not found / 找不到编译器'}
$encoder=$null
if($FFmpegPath){$encoder=(Resolve-Path -LiteralPath $FFmpegPath).Path;if(!(Test-Path -LiteralPath $encoder -PathType Leaf)){throw 'Invalid FFmpeg file'}}
New-Item -ItemType Directory -Path $OutDir -Force | Out-Null
$source=Join-Path $root 'src 源码'
$copySource=Join-Path $source '随机复制工具 Random Copier'
$joinSource=Join-Path $source '语音拼接工具 Voice Joiner'
$common=@('/nologo','/target:winexe','/reference:System.Windows.Forms.dll','/reference:System.Drawing.dll')
$copyArgs=$common+@(('/out:'+(Join-Path $OutDir '随机音频复制工具 Random Audio Copier.exe')),(Join-Path $copySource '主程序 Program.cs'),(Join-Path $copySource '语言资源 Language.cs'))
& $compiler @copyArgs
if($LASTEXITCODE -ne 0){throw 'Copy tool build failed / 随机复制工具编译失败'}
$joinArgs=$common+@(('/out:'+(Join-Path $OutDir '前奏语音拼接工具 Intro Voice Joiner.exe')),(Join-Path $joinSource '主程序 Program.cs'),(Join-Path $joinSource '语言资源 Language.cs'))
if($encoder){$joinArgs+=('/resource:'+ $encoder+',ffmpeg')}
& $compiler @joinArgs
if($LASTEXITCODE -ne 0){throw 'Joiner build failed / 拼接工具编译失败'}
$mode=if($encoder){'Embedded FFmpeg / 已内置 FFmpeg'}else{'External FFmpeg required beside the joiner or via PEAK_AUDIO_FFMPEG / 需在拼接工具旁放置 ffmpeg.exe 或设置环境变量'}
$mode | Set-Content -LiteralPath (Join-Path $OutDir '音频组件模式 Audio Component Mode.txt') -Encoding UTF8
Get-ChildItem -LiteralPath $OutDir -Filter '*.exe' | ForEach-Object {[pscustomobject]@{File=$_.Name;Bytes=$_.Length;SHA256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash}} | Export-Csv -LiteralPath (Join-Path $OutDir '程序校验 Application Checksums.csv') -NoTypeInformation -Encoding UTF8
Write-Host ('Build OK / 构建成功: '+$OutDir)
