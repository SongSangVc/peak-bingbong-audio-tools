using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

static class Program {
 [STAThread] static void Main(){Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false); Application.Run(new MainForm());}
}
class Plan {
 public string[] Files; public long OriginalBytes, Budget, Used, Count, Cycles; public int[] Order; public int Tail;
 public static Plan Build(string[] files,long budget,int seed){
  var p=new Plan {Files=files,Budget=budget,Order=Enumerable.Range(0,files.Length).ToArray()};
  p.OriginalBytes=files.Sum(f=>new FileInfo(f).Length);
  if(p.OriginalBytes<=0)throw new Exception(L.T("没有非空音频文件。"));
  p.Cycles=budget/p.OriginalBytes; p.Count=checked(p.Cycles*files.Length);p.Used=p.Cycles*p.OriginalBytes;
  var rng=new Random(seed); Shuffle(p.Order,rng);
  // Full rounds always contain every source once. Partial final round uses a seeded order.
  for(int i=0;i<p.Order.Length;i++){long size=new FileInfo(files[p.Order[i]]).Length;if(size>budget-p.Used)break;p.Used+=size;p.Tail++;p.Count++;}
  return p;
 }
 public static void Shuffle(int[] order,Random rng){for(int i=order.Length-1;i>0;i--){int j=rng.Next(i+1),t=order[i];order[i]=order[j];order[j]=t;}}
}
class MainForm:Form {
 ComboBox language=new ComboBox {DropDownStyle=ComboBoxStyle.DropDownList};

 TextBox source=new TextBox(),dest=new TextBox(),seed=new TextBox {Text="12345"}; NumericUpDown capacity=new NumericUpDown {Minimum=1,Maximum=100000000,Value=32,DecimalPlaces=2};
 ComboBox units=new ComboBox {DropDownStyle=ComboBoxStyle.DropDownList},format=new ComboBox {DropDownStyle=ComboBoxStyle.DropDownList};
 CheckBox recursive=new CheckBox {Text=L.T("包含子文件夹"),Checked=true,AutoSize=true}; Button scan=new Button {Text=L.T("计算数量"),AutoSize=true},generate=new Button {Text=L.T("开始复制"),AutoSize=true},cancel=new Button {Text=L.T("停止"),Enabled=false,AutoSize=true};
 Label info=new Label {AutoSize=true,MaximumSize=new Size(740,0),Text=L.T("选择音频目录和输出位置后，点击计算数量。")};ProgressBar progress=new ProgressBar {Dock=DockStyle.Fill}; CancellationTokenSource stop;
 static readonly string[] Extensions={".mp3",".wav",".flac",".ogg",".m4a",".aac",".wma",".aif",".aiff",".opus",".ape",".alac"};
 public MainForm(){Text=L.T("音频伪随机复制工具");ClientSize=new Size(940,560);MinimumSize=new Size(955,600);Font=new Font("Microsoft YaHei UI",10);StartPosition=FormStartPosition.CenterScreen;
 var grid=new TableLayoutPanel {Dock=DockStyle.Fill,Padding=new Padding(18),ColumnCount=3,RowCount=0};grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,150));grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,100));Controls.Add(grid);
 language.Items.AddRange(new object[]{"中文","English"});language.SelectedIndex=L.English?1:0;Row(grid,"Language / 语言",language,null,42);
 var b1=new Button {Text=L.T("浏览…"),Dock=DockStyle.Fill};var b2=new Button {Text=L.T("浏览…"),Dock=DockStyle.Fill};Row(grid,L.T("原音频目录"),source,b1,42);Row(grid,L.T("输出父目录"),dest,b2,42);
 var sizePanel=new FlowLayoutPanel {Dock=DockStyle.Fill};capacity.Width=160;units.Items.AddRange(new object[]{L.T("MiB（1024² 字节）"),L.T("GiB（1024³ 字节）")});units.SelectedIndex=0;units.Width=200;sizePanel.Controls.Add(capacity);sizePanel.Controls.Add(units);Row(grid,L.T("容量上限"),sizePanel,null,44);
 format.Items.Add(L.T("全部音频格式"));format.Items.AddRange(Extensions.Cast<object>().ToArray());format.SelectedIndex=0;Row(grid,L.T("原文件格式"),format,null,42);Row(grid,L.T("随机种子"),seed,null,42);Row(grid,L.T("扫描范围"),recursive,null,35);
 var buttons=new FlowLayoutPanel {Dock=DockStyle.Fill};buttons.Controls.AddRange(new Control[]{scan,generate,cancel});Row(grid,"",buttons,null,45);Row(grid,"",progress,null,24);Row(grid,"",info,null,100);
 language.SelectedIndexChanged+=(sender,args)=>{L.English=language.SelectedIndex==1;L.Save();L.Apply(this,language,info);info.Text=L.T("选择音频目录和输出位置后，点击计算数量。");};L.Apply(this,language,info);
 b1.Click+=(s,e)=>Browse(source);b2.Click+=(s,e)=>Browse(dest);scan.Click+=(s,e)=>{try{ShowPlan(GetPlan());}catch(Exception ex){MessageBox.Show(this,ex.Message,L.T("无法计算"));}};generate.Click+=async(s,e)=>await Run();cancel.Click+=(s,e)=>{if(stop!=null)stop.Cancel();};FormClosing+=(s,e)=>{if(stop!=null){stop.Cancel();e.Cancel=true;}};
 }
 void Browse(TextBox box){using(var d=new FolderBrowserDialog()){if(Directory.Exists(box.Text))d.SelectedPath=box.Text;if(d.ShowDialog(this)==DialogResult.OK){box.Text=d.SelectedPath;if(box==source&&dest.Text.Length==0)dest.Text=Path.Combine(d.SelectedPath,L.T("伪随机输出"));}}}
 void Row(TableLayoutPanel g,string title,Control control,Control button,int height){int row=g.RowCount++;g.RowStyles.Add(new RowStyle(SizeType.Absolute,height));g.Controls.Add(new Label {Text=title,AutoSize=true,Anchor=AnchorStyles.Left},0,row);control.Dock=DockStyle.Fill;g.Controls.Add(control,1,row);if(button!=null)g.Controls.Add(button,2,row);else g.SetColumnSpan(control,2);}
 int Seed(){int n;if(int.TryParse(seed.Text,out n))return n;unchecked{n=23;foreach(char c in seed.Text)n=n*31+c;return n;}}
 Plan GetPlan(){if(!Directory.Exists(source.Text))throw new Exception(L.T("请选择存在的原音频目录。"));if(string.IsNullOrWhiteSpace(dest.Text))throw new Exception(L.T("请选择输出父目录。"));string output=Path.GetFullPath(dest.Text).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;string input=Path.GetFullPath(source.Text).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;if(input.Equals(output,StringComparison.OrdinalIgnoreCase))throw new Exception(L.T("输出父目录请与原音频目录分开。"));var list=new List<string>();Collect(input,output,list);var files=list.Where(f=>new FileInfo(f).Length>0).OrderBy(f=>f,StringComparer.OrdinalIgnoreCase).ToArray();if(files.Length==0)throw new Exception(L.T("没有符合所选格式的非空音频。"));long bytes=checked((long)(capacity.Value*(units.SelectedIndex==0?1048576m:1073741824m)));return Plan.Build(files,bytes,Seed());}
 void Collect(string root,string excluded,List<string> list){foreach(var f in Directory.GetFiles(root)){string ext=Path.GetExtension(f).ToLowerInvariant();if(Extensions.Contains(ext)&&(format.SelectedIndex==0||ext==(string)format.SelectedItem))list.Add(f);}if(recursive.Checked)foreach(var d in Directory.GetDirectories(root)){if((File.GetAttributes(d)&FileAttributes.ReparsePoint)!=0)continue;string path=Path.GetFullPath(d).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;if(path.StartsWith(excluded,StringComparison.OrdinalIgnoreCase)||File.Exists(Path.Combine(d,".pseudo-audio-output")))continue;Collect(d,excluded,list);}}
 void ShowPlan(Plan p){info.Text=string.Format(L.T("原音频：{0:N0} 个，共 {1:N2} MiB\n可复制：{2:N0} 个，共 {3:N2} MiB；剩余 {4:N0} 字节\n{5:N0} 个完整轮次 + {6} 个文件；每轮打乱顺序，保留原扩展名。"),p.Files.Length,p.OriginalBytes/1048576.0,p.Count,p.Used/1048576.0,p.Budget-p.Used,p.Cycles,p.Tail);}
 async Task Run(){try{var p=GetPlan();ShowPlan(p);if(p.Count==0){MessageBox.Show(this,L.T("容量不足以容纳下一条完整音频，请增加容量。"));return;}string parent=Path.GetFullPath(dest.Text);Directory.CreateDirectory(parent);string output=Path.Combine(parent,L.T("音频_")+DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"));Directory.CreateDirectory(output);File.WriteAllText(Path.Combine(output,".pseudo-audio-output"),"Audio copy output",Encoding.UTF8);int randomSeed=Seed();stop=new CancellationTokenSource();var token=stop.Token;language.Enabled=false;generate.Enabled=scan.Enabled=false;cancel.Enabled=true;progress.Value=0;long done=0;bool cancelled=false;
 try{await Task.Run(()=>{var rng=new Random(randomSeed);int[] order=Enumerable.Range(0,p.Files.Length).ToArray();for(long round=0;round<p.Cycles;round++){Plan.Shuffle(order,rng);foreach(int i in order){token.ThrowIfCancellationRequested();Copy(p.Files[i],Path.Combine(output,OutputName(done+1,p.Files[i])),token);done++;UpdateProgress(done,p.Count);}}foreach(int i in p.Order.Take(p.Tail)){token.ThrowIfCancellationRequested();Copy(p.Files[i],Path.Combine(output,OutputName(done+1,p.Files[i])),token);done++;UpdateProgress(done,p.Count);}});}catch(OperationCanceledException){cancelled=true;}finally{stop.Dispose();stop=null;language.Enabled=true;generate.Enabled=scan.Enabled=true;cancel.Enabled=false;}info.Text=string.Format(L.T("{0}：已复制 {1:N0} 个文件。\n{2}"),cancelled?L.T("已停止"):L.T("完成"),done,output);MessageBox.Show(this,info.Text,L.T("音频复制"));}
 catch(Exception ex){if(stop!=null){stop.Dispose();stop=null;}language.Enabled=true;generate.Enabled=scan.Enabled=true;cancel.Enabled=false;MessageBox.Show(this,ex.Message,L.T("复制失败"));}}
 void UpdateProgress(long done,long total){if(done%100==0||done==total)BeginInvoke((Action)(()=>{progress.Value=(int)(done*100.0/total);info.Text=string.Format(L.T("正在复制：{0:N0} / {1:N0}"),done,total);}));}
 public static string OutputName(long index,string source){return index.ToString(System.Globalization.CultureInfo.InvariantCulture)+"_"+Path.GetFileName(source);}
 static void Copy(string input,string output,CancellationToken token){try{using(var r=File.OpenRead(input))using(var w=new FileStream(output,FileMode.CreateNew,FileAccess.Write)){byte[] buffer=new byte[1024*1024];int n;while((n=r.Read(buffer,0,buffer.Length))>0){token.ThrowIfCancellationRequested();w.Write(buffer,0,n);}}}catch{if(File.Exists(output))File.Delete(output);throw;}}
}
