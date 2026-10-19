[CmdletBinding()]
param([string]$FFmpegPath,[string]$KitPath,[switch]$IncludeAudio,[string]$Version='0.1.0')
$ErrorActionPreference='Stop'
if($Version -notmatch '^[a-zA-Z0-9._-]+$'){throw 'Invalid version'}
$root=Split-Path -Parent $PSScriptRoot
if($IncludeAudio){if($KitPath){$audioSource=Join-Path (Resolve-Path -LiteralPath $KitPath).Path '02_音频素材 Audio Assets'}else{$audioSource=Join-Path $root 'audio 音频'};if(!(Test-Path -LiteralPath $audioSource)){throw 'Audio folder missing'}}
$stage=Join-Path ([IO.Path]::GetTempPath()) ('PeakRelease_'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $stage | Out-Null
$app=Join-Path $stage '01_实用程序 Applications'
& (Join-Path $PSScriptRoot '构建 Build.ps1') -FFmpegPath $FFmpegPath -OutDir $app
Copy-Item -LiteralPath (Join-Path $root 'docs 文档') -Destination (Join-Path $stage 'docs 文档') -Recurse
Copy-Item -LiteralPath (Join-Path $root 'scripts 脚本') -Destination (Join-Path $stage 'scripts 脚本') -Recurse
Copy-Item -LiteralPath (Join-Path $root 'src 源码') -Destination (Join-Path $stage 'src 源码') -Recurse
Copy-Item -LiteralPath (Join-Path $root 'third-party 第三方组件') -Destination (Join-Path $stage 'third-party 第三方组件') -Recurse
foreach($file in @('README.md','README.en.md','CONTACT.md','COPYRIGHT.md','LICENSING.md','CHANGELOG.md')){Copy-Item -LiteralPath (Join-Path $root $file) -Destination $stage}
if($IncludeAudio){Copy-Item -LiteralPath $audioSource -Destination (Join-Path $stage '02_音频素材 Audio Assets') -Recurse}
# Keep audio links useful in both full-kit and tools-only archives.
foreach($markdown in Get-ChildItem -LiteralPath $stage -Recurse -File -Filter '*.md'){
 $text=[IO.File]::ReadAllText($markdown.FullName)
 if($IncludeAudio){$text=$text.Replace('audio%20音频/','02_音频素材%20Audio%20Assets/')}
 else{$text=$text.Replace('../audio%20音频/','https://github.com/SongSangVc/peak-bingbong-audio-tools/tree/main/audio%20%E9%9F%B3%E9%A2%91/').Replace('(audio%20音频/','(https://github.com/SongSangVc/peak-bingbong-audio-tools/tree/main/audio%20%E9%9F%B3%E9%A2%91/')}
 [IO.File]::WriteAllText($markdown.FullName,$text,[Text.UTF8Encoding]::new($false))
}
$manifest=Join-Path $stage '完整文件校验 Complete File Checksums.csv'
$entries=@(Get-ChildItem -LiteralPath $stage -Recurse -File | Where-Object {$_.FullName -ne $manifest} | ForEach-Object {[pscustomobject]@{RelativePath=$_.FullName.Substring($stage.Length+1).Replace('\','/');Bytes=$_.Length;SHA256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash}})
$entries | Export-Csv -LiteralPath $manifest -NoTypeInformation -Encoding UTF8
$releases=Join-Path $root 'releases 本地发布'
New-Item -ItemType Directory -Path $releases -Force | Out-Null
$kind=if($IncludeAudio){'完整资料包 Full Kit'}else{'工具包 Tools'}
$archive=Join-Path $releases ('PEAK宾邦 '+$kind+' v'+$Version+'.zip')
if(Test-Path -LiteralPath $archive){throw ('Archive already exists / 压缩包已存在: '+$archive)}
Add-Type -AssemblyName System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::CreateFromDirectory($stage,$archive,[IO.Compression.CompressionLevel]::Optimal,$false)
(Get-FileHash -LiteralPath $archive -Algorithm SHA256).Hash+'  '+[IO.Path]::GetFileName($archive) | Set-Content -LiteralPath ($archive+'.sha256') -Encoding UTF8
Write-Host ('Release prepared locally / 已生成本地发布包: '+$archive)
