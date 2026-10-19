using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
static class L {
 public static volatile bool English=Load();
 static string Config {get{return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"PeakAudioTools","language.txt");}}
 static bool Load(){try{return File.ReadAllText(Config).Trim()=="en";}catch{return false;}}
 public static void Save(){try{Directory.CreateDirectory(Path.GetDirectoryName(Config));File.WriteAllText(Config,English?"en":"zh");}catch{}}
 static readonly Dictionary<string,string> Map=new Dictionary<string,string>{
{"没有非空音频文件。","No non-empty audio files found."},
{"包含子文件夹","Include subfolders"},
{"计算数量","Calculate count"},
{"开始复制","Start copying"},
{"停止","Stop"},
{"选择音频目录和输出位置后，点击计算数量。","Choose the source and output folders, then calculate the count."},
{"音频伪随机复制工具","Audio Random Copy"},
{"浏览…","Browse..."},
{"原音频目录","Source folder"},
{"输出父目录","Output folder"},
{"MiB（1024² 字节）","MiB (1024² bytes)"},
{"GiB（1024³ 字节）","GiB (1024³ bytes)"},
{"容量上限","Capacity limit"},
{"全部音频格式","All audio formats"},
{"原文件格式","Source format"},
{"随机种子","Random seed"},
{"扫描范围","Scan options"},
{"无法计算","Cannot calculate"},
{"伪随机输出","RandomOutput"},
{"请选择存在的原音频目录。","Please choose an existing source folder."},
{"请选择输出父目录。","Please choose an output folder."},
{"输出父目录请与原音频目录分开。","Choose an output folder different from the source folder."},
{"没有符合所选格式的非空音频。","No non-empty audio files match the selected format."},
{"原音频：{0:N0} 个，共 {1:N2} MiB\n可复制：{2:N0} 个，共 {3:N2} MiB；剩余 {4:N0} 字节\n{5:N0} 个完整轮次 + {6} 个文件；每轮打乱顺序，保留原扩展名。","Source: {0:N0} files, {1:N2} MiB\nOutput: {2:N0} files, {3:N2} MiB; {4:N0} bytes remaining\n{5:N0} full rounds + {6} files; each round shuffled; original names retained."},
{"容量不足以容纳下一条完整音频，请增加容量。","The next complete audio file does not fit. Increase the capacity."},
{"音频_","Audio_"},
{"{0}：已复制 {1:N0} 个文件。\n{2}","{0}: copied {1:N0} files.\n{2}"},
{"已停止","Stopped"},
{"完成","Completed"},
{"音频复制","Audio copy"},
{"复制失败","Copy failed"},
{"正在复制：{0:N0} / {1:N0}","Copying: {0:N0} / {1:N0}"},
{"前奏随机范围：下限不能大于上限。","Intro random range: minimum must not exceed maximum."},
{"语音随机范围：下限不能大于上限。","Voice random range: minimum must not exceed maximum."},
{"前奏处理={0} 音量={1:F5} 音高={2:F5}；语音处理={3} 音量={4:F5} 音高={5:F5}","Intro effects={0} volume={1:F5} pitch={2:F5}; voice effects={3} volume={4:F5} pitch={5:F5}"},
{"前奏随机音量=","Intro random volume="},
{"] 随机音高=","] random pitch="},
{"]；语音随机音量=","]; voice random volume="},
{"找不到内置音频组件。","The built-in audio component was not found."},
{"启用前奏处理","Enable intro effects"},
{"启用语音处理","Enable voice effects"},
{"随机音量","Random volume"},
{"随机音高","Random pitch"},
{"开始拼接","Start joining"},
{"每条语音生成一个文件；多前奏按随机种子分配。","One output per voice; multiple intros are selected using the random seed."},
{"语音与前奏拼接工具 · 随机效果版","Intro + Voice Joiner - Random Effects"},
{"语音目录","Voice folder"},
{"单个前奏文件","Single intro file"},
{"文件夹内随机前奏","Random intro from folder"},
{"前奏来源","Intro source"},
{"前奏路径","Intro path"},
{"音频|*.mp3;*.wav;*.flac;*.ogg;*.m4a;*.aac;*.wma;*.opus;*.aif;*.aiff|所有文件|*.*","Audio|*.mp3;*.wav;*.flac;*.ogg;*.m4a;*.aac;*.wma;*.opus;*.aif;*.aiff|All files|*.*"},
{"静音时长（秒）","Silence (seconds)"},
{"前奏前","Before intro"},
{"前奏与语音之间","Between intro and voice"},
{"前奏处理","Intro effects"},
{"音量倍数","Volume multiplier"},
{"音高倍数","Pitch multiplier"},
{"前奏随机音量","Intro random volume"},
{"前奏随机音高","Intro random pitch"},
{"语音处理","Voice effects"},
{"语音随机音量","Voice random volume"},
{"语音随机音高","Voice random pitch"},
{"处理说明","Effect notes"},
{"随机勾选后，范围取代该项固定倍数；每条独立均匀取值。须先启用对应处理。\n音高调整也改变速度和时长；音量 1× 为原音量。","Random ranges replace fixed multipliers; sampled uniformly per output. Enable effects first.\nPitch also changes playback speed and duration; volume 1x is the original level."},
{"输出格式","Output format"},
{"下限（倍）","Minimum (x)"},
{"上限（倍）","Maximum (x)"},
{"请选择有效语音目录。","Please choose a valid voice folder."},
{"语音目录中没有音频。","No audio files found in the voice folder."},
{"请选择有效前奏文件。","Please choose a valid intro file."},
{"请选择有效前奏目录。","Please choose a valid intro folder."},
{"前奏目录中没有音频。","No audio files found in the intro folder."},
{"拼接_","Joined_"},
{"拼接记录.txt","JoinLog.txt"},
{"前奏前静音=","Silence before intro="},
{" 秒；间隔="," sec; gap="},
{" 秒；随机种子="," sec; random seed="},
{"前奏处理=","Intro effects="},
{" 音量="," volume="},
{" 音高="," pitch="},
{"；语音处理=","; voice effects="},
{"\t前奏=","\tIntro="},
{"\t语音=","\tVoice="},
{"失败：","Failed: "},
{"已处理 ","Processed "},
{"：成功 ",": succeeded "},
{" 条，失败 ","; failed "},
{" 条。\n",".\n"},
{"错误记录.txt","Errors.txt"},
{"拼接结果","Join results"},
{"无法拼接","Cannot join"}
 };
 public static string T(string text){string value;return English&&Map.TryGetValue(text,out value)?value:text;}
 static string Original(string text){if(Map.ContainsKey(text))return text;foreach(var pair in Map)if(pair.Value==text)return pair.Key;return text;}
 static readonly Dictionary<ComboBox,string[]> Options=new Dictionary<ComboBox,string[]>();
 public static void Apply(Control root,Control language,Control status){
 if(root==language||root==status)return;
 var combo=root as ComboBox;
 if(combo!=null){string[] entries;if(!Options.TryGetValue(combo,out entries)){entries=new string[combo.Items.Count];for(int i=0;i<entries.Length;i++)entries[i]=Original(Convert.ToString(combo.Items[i]));Options[combo]=entries;}int selected=combo.SelectedIndex;combo.BeginUpdate();for(int i=0;i<entries.Length;i++)combo.Items[i]=T(entries[i]);combo.SelectedIndex=selected;combo.EndUpdate();}
 else if(root is Label||root is Button||root is CheckBox||root is Form){if(root.Tag==null)root.Tag=Original(root.Text);root.Text=T((string)root.Tag);}
 foreach(Control child in root.Controls)Apply(child,language,status);
 }
}
