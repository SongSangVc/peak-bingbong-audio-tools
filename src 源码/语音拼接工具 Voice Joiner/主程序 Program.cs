using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

static class Program {
 [STAThread] static void Main(string[] args){Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);Application.Run(new MainForm());}
}
class Settings {
 public double Lead,Gap,IntroVolume=1,VoiceVolume=1,IntroPitch=1,VoicePitch=1;public bool IntroProcess,VoiceProcess;public string Format="mp3";
 public bool RandomIV,RandomIP,RandomVV,RandomVP;
 public double IVMin=0.9,IVMax=1.1,IPMin=0.9,IPMax=1.1,VVMin=0.9,VVMax=1.1,VPMin=0.9,VPMax=1.1;
 public Settings Sample(Random rng){var s=(Settings)MemberwiseClone();
 if(IntroProcess){if(RandomIV)s.IntroVolume=Between(rng,IVMin,IVMax);if(RandomIP)s.IntroPitch=Between(rng,IPMin,IPMax);}
 if(VoiceProcess){if(RandomVV)s.VoiceVolume=Between(rng,VVMin,VVMax);if(RandomVP)s.VoicePitch=Between(rng,VPMin,VPMax);}return s;}
 static double Between(Random rng,double min,double max){return min+(max-min)*rng.NextDouble();}
 public void Validate(){if(IntroProcess&&((RandomIV&&IVMin>IVMax)||(RandomIP&&IPMin>IPMax)))throw new Exception(L.T("前奏随机范围：下限不能大于上限。"));if(VoiceProcess&&((RandomVV&&VVMin>VVMax)||(RandomVP&&VPMin>VPMax)))throw new Exception(L.T("语音随机范围：下限不能大于上限。"));}
 public string Description(){return string.Format(CultureInfo.InvariantCulture,L.T("前奏处理={0} 音量={1:F5} 音高={2:F5}；语音处理={3} 音量={4:F5} 音高={5:F5}"),IntroProcess,IntroProcess?IntroVolume:1,IntroProcess?IntroPitch:1,VoiceProcess,VoiceProcess?VoiceVolume:1,VoiceProcess?VoicePitch:1);}
 public string Ranges(){return L.T("前奏随机音量=")+RandomIV+" ["+IVMin+", "+IVMax+L.T("] 随机音高=")+RandomIP+" ["+IPMin+", "+IPMax+L.T("]；语音随机音量=")+RandomVV+" ["+VVMin+", "+VVMax+L.T("] 随机音高=")+RandomVP+" ["+VPMin+", "+VPMax+"]";}

}
static class Audio {
 static string tool;static bool ownsTool;
 static readonly object sync=new object();
 public static string Tool(){lock(sync){
 if(tool!=null&&File.Exists(tool))return tool;
 string configured=Environment.GetEnvironmentVariable("PEAK_AUDIO_FFMPEG");
 if(!string.IsNullOrWhiteSpace(configured)){if(!File.Exists(configured))throw new Exception(L.T("PEAK_AUDIO_FFMPEG 指向的文件不存在。"));return tool=Path.GetFullPath(configured);}
 using(var r=Assembly.GetExecutingAssembly().GetManifestResourceStream("ffmpeg")){
 if(r==null){string beside=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"ffmpeg.exe");if(File.Exists(beside))return tool=beside;throw new Exception(L.T("请将 ffmpeg.exe 放在程序旁，或设置 PEAK_AUDIO_FFMPEG。"));}
 string dir=Path.Combine(Path.GetTempPath(),"AudioConcat_"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(dir);string extracted=Path.Combine(dir,"ffmpeg.exe");
 try{using(var w=File.Create(extracted))r.CopyTo(w);tool=extracted;ownsTool=true;return tool;}catch{try{Directory.Delete(dir,true);}catch{}throw;}
 }}}
 public static void Cleanup(){lock(sync){try{if(ownsTool&&tool!=null)Directory.Delete(Path.GetDirectoryName(tool),true);}catch{}tool=null;ownsTool=false;}}
 static string N(double n){return n.ToString("0.########",CultureInfo.InvariantCulture);}
 static string Q(string s){return "\""+s.Replace("\"","\\\"")+"\"";}
 static string Chain(bool on,double volume,double pitch){return "aresample=48000,aformat=sample_fmts=fltp:channel_layouts=stereo"+(on?",asetrate="+N(48000*pitch)+",aresample=48000,volume="+N(volume):"")+",asetpts=PTS-STARTPTS";}
 public static string Filter(Settings s){string f="[0:a:0]"+Chain(s.IntroProcess,s.IntroVolume,s.IntroPitch)+"[intro];[1:a:0]"+Chain(s.VoiceProcess,s.VoiceVolume,s.VoicePitch)+"[voice];";string inputs="";int count=2;if(s.Lead>0){f+="anullsrc=r=48000:cl=stereo,atrim=duration="+N(s.Lead)+",asetpts=PTS-STARTPTS[lead];";inputs+="[lead]";count++;}inputs+="[intro]";if(s.Gap>0){f+="anullsrc=r=48000:cl=stereo,atrim=duration="+N(s.Gap)+",asetpts=PTS-STARTPTS[gap];";inputs+="[gap]";count++;}return f+inputs+"[voice]concat=n="+count+":v=0:a=1[out]";}
 public static void Render(string intro,string voice,string output,Settings s,CancellationToken token){string codec=s.Format=="wav"?"-c:a pcm_s16le":s.Format=="flac"?"-c:a flac":"-c:a libmp3lame -q:a 2";string pending=Path.Combine(Path.GetDirectoryName(output),Guid.NewGuid().ToString("N")+".partial."+s.Format);try{var start=new ProcessStartInfo(Tool(),"-nostdin -hide_banner -loglevel error -y -i "+Q(intro)+" -i "+Q(voice)+" -filter_complex "+Q(Filter(s))+" -map "+Q("[out]")+" -vn -ar 48000 "+codec+" "+Q(pending)){UseShellExecute=false,CreateNoWindow=true,RedirectStandardError=true};using(var p=Process.Start(start)){var error=p.StandardError.ReadToEndAsync();while(!p.WaitForExit(150)){if(token.IsCancellationRequested){try{p.Kill();}catch{}p.WaitForExit();token.ThrowIfCancellationRequested();}}string message=error.GetAwaiter().GetResult();token.ThrowIfCancellationRequested();if(p.ExitCode!=0)throw new Exception(message.Length>1600?message.Substring(0,1600):message);}File.Move(pending,output);}finally{if(File.Exists(pending))File.Delete(pending);}}
}
class MainForm:Form {
 ComboBox language=new ComboBox {DropDownStyle=ComboBoxStyle.DropDownList};

 TextBox voices=new TextBox(),intro=new TextBox(),output=new TextBox();ComboBox introMode=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList},format=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList};
 NumericUpDown lead=Number(0.1m,0,600,2),gap=Number(1,0,600,2),iv=Number(1,0,4,2),ip=Number(1,0.25m,4,2),vv=Number(1,0,4,2),vp=Number(1,0.25m,4,2),seed=Number(12345,0,int.MaxValue,0);
 CheckBox introProcess=new CheckBox{Text=L.T("启用前奏处理"),AutoSize=true},voiceProcess=new CheckBox{Text=L.T("启用语音处理"),AutoSize=true},recursive=new CheckBox{Text=L.T("包含子文件夹"),AutoSize=true};
 CheckBox riv=new CheckBox{Text=L.T("随机音量"),AutoSize=true},rip=new CheckBox{Text=L.T("随机音高"),AutoSize=true},rvv=new CheckBox{Text=L.T("随机音量"),AutoSize=true},rvp=new CheckBox{Text=L.T("随机音高"),AutoSize=true};
 NumericUpDown ivMin=Number(0.9m,0,4,2),ivMax=Number(1.1m,0,4,2),ipMin=Number(0.9m,0.25m,4,2),ipMax=Number(1.1m,0.25m,4,2),vvMin=Number(0.9m,0,4,2),vvMax=Number(1.1m,0,4,2),vpMin=Number(0.9m,0.25m,4,2),vpMax=Number(1.1m,0.25m,4,2);
 Button run=new Button{Text=L.T("开始拼接"),AutoSize=true},cancel=new Button{Text=L.T("停止"),Enabled=false,AutoSize=true};ProgressBar progress=new ProgressBar{Dock=DockStyle.Fill};Label status=new Label{Text=L.T("每条语音生成一个文件；多前奏按随机种子分配。"),AutoSize=true,MaximumSize=new Size(850,0)};CancellationTokenSource stop;
 static NumericUpDown Number(decimal val,decimal min,decimal max,int digits){return new NumericUpDown{Minimum=min,Maximum=max,Value=val,DecimalPlaces=digits,Increment=digits==0?1:0.1m,Width=85};}
 public MainForm(){Text=L.T("语音与前奏拼接工具 · 随机效果版");Font=new Font("Microsoft YaHei UI",10);ClientSize=new Size(1100,890);MinimumSize=new Size(1115,600);AutoScroll=true;StartPosition=FormStartPosition.CenterScreen;var grid=new TableLayoutPanel{Dock=DockStyle.Top,AutoSize=true,Padding=new Padding(18),ColumnCount=3,RowCount=0};grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,175));grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,90));Controls.Add(grid);
 language.Items.AddRange(new object[]{"中文","English"});language.SelectedIndex=L.English?1:0;Row(grid,"Language / 语言",language,null,42);
 Row(grid,L.T("语音目录"),voices,BrowseButton(()=>BrowseFolder(voices)),43);introMode.Items.AddRange(new object[]{L.T("单个前奏文件"),L.T("文件夹内随机前奏")});introMode.SelectedIndex=0;Row(grid,L.T("前奏来源"),introMode,null,42);Row(grid,L.T("前奏路径"),intro,BrowseButton(()=>{if(introMode.SelectedIndex==1)BrowseFolder(intro);else using(var d=new OpenFileDialog{Filter=L.T("音频|*.mp3;*.wav;*.flac;*.ogg;*.m4a;*.aac;*.wma;*.opus;*.aif;*.aiff|所有文件|*.*")}){if(d.ShowDialog(this)==DialogResult.OK)intro.Text=d.FileName;}}),43);Row(grid,L.T("输出父目录"),output,BrowseButton(()=>BrowseFolder(output)),43);
 Row(grid,L.T("静音时长（秒）"),Flow(new Label{Text=L.T("前奏前"),AutoSize=true},lead,new Label{Text=L.T("前奏与语音之间"),AutoSize=true},gap),null,43);
 Row(grid,L.T("前奏处理"),Flow(introProcess,new Label{Text=L.T("音量倍数"),AutoSize=true},iv,new Label{Text=L.T("音高倍数"),AutoSize=true},ip),null,43);
 Row(grid,L.T("前奏随机音量"),RandomRow(riv,ivMin,ivMax),null,40);
 Row(grid,L.T("前奏随机音高"),RandomRow(rip,ipMin,ipMax),null,40);
 Row(grid,L.T("语音处理"),Flow(voiceProcess,new Label{Text=L.T("音量倍数"),AutoSize=true},vv,new Label{Text=L.T("音高倍数"),AutoSize=true},vp),null,43);
 Row(grid,L.T("语音随机音量"),RandomRow(rvv,vvMin,vvMax),null,40);
 Row(grid,L.T("语音随机音高"),RandomRow(rvp,vpMin,vpMax),null,40);
 Row(grid,L.T("处理说明"),new Label{Text=L.T("随机勾选后，范围取代该项固定倍数；每条独立均匀取值。须先启用对应处理。\n音高调整也改变速度和时长；音量 1× 为原音量。"),AutoSize=true},null,55);
 format.Items.AddRange(new object[]{"mp3","wav","flac"});format.SelectedIndex=0;Row(grid,L.T("输出格式"),format,null,40);Row(grid,L.T("随机种子"),Flow(seed,recursive),null,40);Row(grid,"",Flow(run,cancel),null,42);Row(grid,"",progress,null,24);Row(grid,"",status,null,70);
 language.SelectedIndexChanged+=(sender,args)=>{L.English=language.SelectedIndex==1;L.Save();L.Apply(this,language,status);status.Text=L.T("每条语音生成一个文件；多前奏按随机种子分配。");};L.Apply(this,language,status);
 introProcess.CheckedChanged+=(s,e)=>UpdateEffects();voiceProcess.CheckedChanged+=(s,e)=>UpdateEffects();foreach(var box in new[]{riv,rip,rvv,rvp})box.CheckedChanged+=(s,e)=>UpdateEffects();UpdateEffects();run.Click+=async(s,e)=>await Run();cancel.Click+=(s,e)=>{if(stop!=null)stop.Cancel();};FormClosing+=(s,e)=>{if(stop!=null){stop.Cancel();e.Cancel=true;}else Audio.Cleanup();};}
 static Control RandomRow(CheckBox box,NumericUpDown min,NumericUpDown max){return Flow(box,new Label{Text=L.T("下限（倍）"),AutoSize=true},min,new Label{Text=L.T("上限（倍）"),AutoSize=true},max);}
 void UpdateEffects(){riv.Enabled=rip.Enabled=introProcess.Checked;rvv.Enabled=rvp.Enabled=voiceProcess.Checked;iv.Enabled=introProcess.Checked&&!riv.Checked;ip.Enabled=introProcess.Checked&&!rip.Checked;vv.Enabled=voiceProcess.Checked&&!rvv.Checked;vp.Enabled=voiceProcess.Checked&&!rvp.Checked;ivMin.Enabled=ivMax.Enabled=introProcess.Checked&&riv.Checked;ipMin.Enabled=ipMax.Enabled=introProcess.Checked&&rip.Checked;vvMin.Enabled=vvMax.Enabled=voiceProcess.Checked&&rvv.Checked;vpMin.Enabled=vpMax.Enabled=voiceProcess.Checked&&rvp.Checked;}
 static FlowLayoutPanel Flow(params Control[] items){var f=new FlowLayoutPanel{Dock=DockStyle.Fill,WrapContents=false};f.Controls.AddRange(items);return f;}
 Button BrowseButton(Action action){var b=new Button{Text=L.T("浏览…"),Dock=DockStyle.Fill};b.Click+=(s,e)=>action();return b;}
 void BrowseFolder(TextBox box){using(var d=new FolderBrowserDialog()){if(Directory.Exists(box.Text))d.SelectedPath=box.Text;if(d.ShowDialog(this)==DialogResult.OK)box.Text=d.SelectedPath;}}
 void Row(TableLayoutPanel grid,string text,Control field,Control button,int height){int row=grid.RowCount++;grid.RowStyles.Add(new RowStyle(SizeType.Absolute,height));grid.Controls.Add(new Label{Text=text,AutoSize=true,Anchor=AnchorStyles.Left},0,row);field.Dock=DockStyle.Fill;grid.Controls.Add(field,1,row);if(button==null)grid.SetColumnSpan(field,2);else grid.Controls.Add(button,2,row);}
 static bool IsAudio(string path){return new[]{".mp3",".wav",".flac",".ogg",".m4a",".aac",".wma",".opus",".aif",".aiff",".ape"}.Contains(Path.GetExtension(path).ToLowerInvariant());}
 static void Collect(string dir,bool deep,string exclude,List<string> files){foreach(string f in Directory.GetFiles(dir))if(IsAudio(f))files.Add(f);if(deep)foreach(string d in Directory.GetDirectories(dir)){if((File.GetAttributes(d)&FileAttributes.ReparsePoint)!=0)continue;if(File.Exists(Path.Combine(d,".audio-concat-output")))continue;if((Path.GetFullPath(d).TrimEnd('\\')+"\\").StartsWith(exclude,StringComparison.OrdinalIgnoreCase))continue;Collect(d,true,exclude,files);}}
 async Task Run(){try{if(!Directory.Exists(voices.Text))throw new Exception(L.T("请选择有效语音目录。"));if(string.IsNullOrWhiteSpace(output.Text))throw new Exception(L.T("请选择输出父目录。"));string parent=Path.GetFullPath(output.Text);string excluded=parent.TrimEnd('\\')+"\\";var voiceList=new List<string>();Collect(voices.Text,recursive.Checked,excluded,voiceList);string[] lines=voiceList.OrderBy(f=>f,StringComparer.OrdinalIgnoreCase).ToArray();if(lines.Length==0)throw new Exception(L.T("语音目录中没有音频。"));var intros=new List<string>();if(introMode.SelectedIndex==0){if(!File.Exists(intro.Text))throw new Exception(L.T("请选择有效前奏文件。"));intros.Add(intro.Text);}else{if(!Directory.Exists(intro.Text))throw new Exception(L.T("请选择有效前奏目录。"));Collect(intro.Text,recursive.Checked,excluded,intros);}if(intros.Count==0)throw new Exception(L.T("前奏目录中没有音频。"));intros.Sort(StringComparer.OrdinalIgnoreCase);
 var settings=new Settings{Lead=(double)lead.Value,Gap=(double)gap.Value,IntroProcess=introProcess.Checked,VoiceProcess=voiceProcess.Checked,IntroVolume=(double)iv.Value,IntroPitch=(double)ip.Value,VoiceVolume=(double)vv.Value,VoicePitch=(double)vp.Value,Format=(string)format.SelectedItem,RandomIV=riv.Checked,RandomIP=rip.Checked,RandomVV=rvv.Checked,RandomVP=rvp.Checked,IVMin=(double)ivMin.Value,IVMax=(double)ivMax.Value,IPMin=(double)ipMin.Value,IPMax=(double)ipMax.Value,VVMin=(double)vvMin.Value,VVMax=(double)vvMax.Value,VPMin=(double)vpMin.Value,VPMax=(double)vpMax.Value};settings.Validate();int randomSeed=(int)seed.Value;Directory.CreateDirectory(parent);string dest=Path.Combine(parent,L.T("拼接_")+DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"));Directory.CreateDirectory(dest);File.WriteAllText(Path.Combine(dest,".audio-concat-output"),"Audio concatenation output");stop=new CancellationTokenSource();var token=stop.Token;language.Enabled=false;run.Enabled=false;cancel.Enabled=true;progress.Value=0;int done=0;bool cancelled=false;var failures=new List<string>();
 try{await Task.Run(()=>{var rng=new Random(randomSeed);var effectsRng=new Random(unchecked(randomSeed ^ 0x51ed270b));var names=new HashSet<string>(StringComparer.OrdinalIgnoreCase);using(var log=new StreamWriter(Path.Combine(dest,L.T("拼接记录.txt")),false,Encoding.UTF8)){log.WriteLine(L.T("前奏前静音=")+settings.Lead+L.T(" 秒；间隔=")+settings.Gap+L.T(" 秒；随机种子=")+randomSeed);log.WriteLine(L.T("前奏处理=")+settings.IntroProcess+L.T(" 音量=")+settings.IntroVolume+L.T(" 音高=")+settings.IntroPitch+L.T("；语音处理=")+settings.VoiceProcess+L.T(" 音量=")+settings.VoiceVolume+L.T(" 音高=")+settings.VoicePitch);log.WriteLine(settings.Ranges());for(int i=0;i<lines.Length;i++){token.ThrowIfCancellationRequested();var sampled=settings.Sample(effectsRng);string prefix=intros[rng.Next(intros.Count)],stem=Path.GetFileNameWithoutExtension(lines[i]);string name=stem+"."+settings.Format;int suffix=2;while(!names.Add(name))name=stem+"_"+(suffix++)+"."+settings.Format;try{Audio.Render(prefix,lines[i],Path.Combine(dest,name),sampled,token);done++;log.WriteLine(name+L.T("\t前奏=")+prefix+L.T("\t语音=")+lines[i]+"\t"+sampled.Description());}catch(OperationCanceledException){throw;}catch(Exception ex){failures.Add(name+"："+ex.Message);log.WriteLine(L.T("失败：")+name+" "+ex.Message);}int current=i+1;BeginInvoke((Action)(()=>{progress.Value=(int)(current*100.0/lines.Length);status.Text=L.T("已处理 ")+current+" / "+lines.Length;}));}}});}catch(OperationCanceledException){cancelled=true;}finally{stop.Dispose();stop=null;language.Enabled=true;run.Enabled=true;cancel.Enabled=false;}status.Text=(cancelled?L.T("已停止"):L.T("完成"))+L.T("：成功 ")+done+L.T(" 条，失败 ")+failures.Count+L.T(" 条。\n")+dest;if(failures.Count>0)File.WriteAllLines(Path.Combine(dest,L.T("错误记录.txt")),failures.ToArray(),Encoding.UTF8);MessageBox.Show(this,status.Text,L.T("拼接结果"));}
 catch(Exception ex){if(stop!=null){stop.Dispose();stop=null;}language.Enabled=true;run.Enabled=true;cancel.Enabled=false;MessageBox.Show(this,ex.Message,L.T("无法拼接"));}}
}
