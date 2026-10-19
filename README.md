# PEAK 宾邦玩偶语音工具 · PEAK Bingbong Audio Plush Tools

[English](README.en.md) · **中文**

维护者：**Song_Sang**（SongSang）

为自制宾邦（Bingbong）发声玩偶准备音频：拼接前奏、调整声音，再按存储容量生成伪随机播放文件集。

这是个人爱好者项目。整个项目使用 **OpenAI Codex** 辅助完成，包括音频提取与处理、工具开发及文档编写。工具面向 **64 位 Windows / .NET Framework 4.x**，界面可切换中英文。

## 为什么这样设计

**让顺序播放呈现近似随机的效果。** 有些玩偶发声器没有随机播放模式。本工具在电脑上预先打乱音频并循环复制，生成容量范围内较长的播放序列，让只支持顺序播放的设备也能听起来更有变化。它不会给硬件增加真正的随机模式；实际播放顺序取决于模块对编号、文件名或写入顺序的要求。

**给发声器留出启动时间。** 前奏前默认加入 `0.1 秒` 静音，用于减少模块启动时漏播音频开头的情况。这个值可按实际模块试听调整：仍有截头就加长，启动快则可缩短或设为 `0`。它与前奏、语音之间的间隔分别设置。

## 两个工具

| 工具 | 能做什么 |
| --- | --- |
| 随机音频复制工具 Random Audio Copier | 按容量上限和输入音频总大小计算文件数量；每轮打乱顺序后循环复制；命名为 `1_原文件名.mp3`、`2_原文件名.mp3`；不改变音频内容。 |
| 前奏语音拼接工具 Intro Voice Joiner | 选择单个前奏或文件夹内随机前奏；设置前奏前与语音前静音；分别启用前奏/语音音量、音高及范围随机；输出 MP3/WAV/FLAC。 |

固定随机种子可以重现排列与效果参数。音高调整会同时改变播放速度和时长；默认随机范围 `0.90～1.10` 是可调整的初始值，并非游戏内实测参数。

## 下载与开始使用

从 [GitHub Releases 下载成品工具和完整资料包](https://github.com/Song-Siyu-05/peak-bingbong-audio-tools/releases/latest)。仓库中已包含整理后的音频；GitHub 的源码 ZIP 包含音频，但不包含编译好的 EXE。

1. 从已发布的 Release 下载工具包，或含素材的完整资料包，并完整解压。
2. 打开 `01_实用程序 Applications`，双击所需 EXE。
3. 用窗口顶部 `Language / 语言` 切换界面。
4. 先用少量音频、较小容量试用，再将结果放到你的音频模块。

完整资料包中的两个 EXE 可以独立移动；**内置 FFmpeg 的拼接版**不需要单独安装音频工具。自行编译的默认版本需外置 FFmpeg，区别见下方构建说明。

- [使用指导 · Bilingual User Guide](docs%20文档/使用指导%20User%20Guide.txt)
- [音频版本说明](docs%20文档/音频素材说明%20Audio%20Assets.md)

## 本地构建

在项目根目录优先使用 **PowerShell 7**（`pwsh`）：

```powershell
& '.\scripts 脚本\构建 Build.ps1'
```

默认输出到 `.build 本地构建/bin`。随机复制工具可直接使用；拼接工具需在 EXE 旁放置 `ffmpeg.exe`，或将 `PEAK_AUDIO_FFMPEG` 环境变量设为该文件的完整路径。

如果希望内置音频组件，构建时指定你自己的 FFmpeg：

```powershell
& '.\scripts 脚本\构建 Build.ps1' -FFmpegPath 'D:\tools\ffmpeg.exe'
pwsh -NoProfile -STA -File '.\scripts 脚本\检查 Check.ps1'
```

构建脚本无需旧版 EXE，不会自动下载组件。更多细节见[构建与发布](docs%20文档/构建与发布%20Build%20and%20Release.md)。

## 项目结构

```text
README.md / README.en.md          中英文项目首页
CONTACT.md / COPYRIGHT.md         反馈、联系方式、音频来源声明
src 源码/                         两个 C# WinForms 程序及语言资源
audio 音频/                       提取的原始音频及调整后的版本
scripts 脚本/                     构建、检查与本地 ZIP 打包
docs 文档/                        用户指南、素材清单、发布说明
third-party 第三方组件/            现有 FFmpeg 组件信息
.github/                          问题反馈模板与 Windows 构建工作流
releases 本地发布/                本地准备的附件；已被 Git 忽略
```

仓库的 [audio 音频目录](audio%20音频/) 已收录提取的原始 MP3 / FSB、两种拼接版本和已有随机复制结果。游戏本体、FFmpeg 二进制及今后生成在其他目录的副本仍被忽略。

## 反馈、来源与许可

邮箱：**SongSang_Yichang@outlook.com**。我只会偶尔查看邮箱，回复可能不及时，也无法保证回复每封邮件。详见[反馈与联系方式](CONTACT.md)。

宾邦语音与相关音效提取自 **PEAK**，权利归原权利人所有；本项目无官方隶属、授权或背书。提取和调整不改变权利归属，免责声明也不构成授权。详见[音频来源与版权](COPYRIGHT.md)。

源码尚未指定开源许可证；公开源码不代表授予 MIT/GPL 等许可。FFmpeg 等第三方组件遵循其各自许可，详见[许可范围](LICENSING.md)及[组件说明](third-party%20第三方组件/README.md)。
