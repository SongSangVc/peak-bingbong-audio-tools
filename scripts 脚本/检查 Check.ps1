[CmdletBinding()]
param([string]$BuildDir)
$ErrorActionPreference='Stop'
if([Threading.Thread]::CurrentThread.GetApartmentState() -ne 'STA'){throw 'Use PowerShell 7: pwsh -STA / 请使用 PowerShell 7 的 pwsh -STA'}
Add-Type -AssemblyName System.Windows.Forms
$root=Split-Path -Parent $PSScriptRoot
if(!$BuildDir){$BuildDir=Join-Path $root '.build 本地构建\bin'}
foreach($file in @('随机音频复制工具 Random Audio Copier.exe','前奏语音拼接工具 Intro Voice Joiner.exe')){
 $assembly=[Reflection.Assembly]::LoadFile((Join-Path $BuildDir $file));$form=$assembly.CreateInstance('MainForm');$type=$form.GetType();$flags=[Reflection.BindingFlags]'NonPublic,Instance';$lang=$type.GetField('language',$flags).GetValue($form);$original=$lang.SelectedIndex
 try{
  $lang.SelectedIndex=0;$zh=$form.Text;$lang.SelectedIndex=1
  if($form.Text -eq $zh -or $form.Text -match '[\u4e00-\u9fff]'){throw 'English translation failed'}
  $lang.SelectedIndex=0;if($form.Text -ne $zh){throw 'Chinese restore failed'}
  if($file -like '*Copier*'){
   $n=$type.GetMethod('OutputName').Invoke($null,@([long]123,[string]'C:\audio\原文件.mp3'));if($n -ne '123_原文件.mp3'){throw 'Filename check failed'}
   $dir=Join-Path ([IO.Path]::GetTempPath()) ('PeakPlan_'+[guid]::NewGuid().ToString('N'));$null=[IO.Directory]::CreateDirectory($dir);$paths=@();foreach($size in @(7,11,13)){$path=Join-Path $dir ($size.ToString()+'.mp3');[IO.File]::WriteAllBytes($path,[byte[]](1..$size));$paths+=$path}
   $planType=$assembly.GetType('Plan');$plan=$planType.GetMethod('Build').Invoke($null,@([string[]]$paths,[long]101,[int]12345));if($planType.GetField('Used').GetValue($plan) -gt 101 -or $planType.GetField('Count').GetValue($plan) -ne 9){throw 'Capacity calculation failed'}
   $argsCopy=New-Object object[] 3;$argsCopy[0]=[string]$paths[0];$argsCopy[1]=[string](Join-Path $dir '1_original.mp3');$argsCopy[2]=[Threading.CancellationToken]::None;$type.GetMethod('Copy',[Reflection.BindingFlags]'NonPublic,Static').Invoke($null,$argsCopy);if((Get-FileHash $paths[0]).Hash -ne (Get-FileHash $argsCopy[1]).Hash){throw 'Copy bytes changed'}
  }else{
   $t=$assembly.GetType('Settings');$s=[Activator]::CreateInstance($t);foreach($f in @('IntroProcess','VoiceProcess','RandomIV','RandomIP','RandomVV','RandomVP')){$t.GetField($f).SetValue($s,$true)};$a1=New-Object object[] 1;$a2=New-Object object[] 1;$a1[0]=[Random]::new(42);$a2[0]=[Random]::new(42)
   for($i=0;$i -lt 100;$i++){$a=$t.GetMethod('Sample').Invoke($s,$a1);$b=$t.GetMethod('Sample').Invoke($s,$a2);foreach($f in @('IntroVolume','IntroPitch','VoiceVolume','VoicePitch')){$v=$t.GetField($f).GetValue($a);if($v -lt 0.9 -or $v -gt 1.1 -or $v -ne $t.GetField($f).GetValue($b)){throw 'Random range or seed check failed'}}}
  }
  Write-Host ('PASS: constructor, languages and behavior / 通过: '+$file)
 }finally{$lang.SelectedIndex=$original;$form.Dispose()}
}
