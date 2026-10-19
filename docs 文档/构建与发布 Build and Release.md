# 构建与发布 · Build and Release

## 环境 / Requirements

64 位 Windows，系统自带的 .NET Framework 4.x 编译器，Windows PowerShell 5.1。脚本兼容本机 PowerShell 7 构建；界面检查请用 Windows PowerShell 的 STA 模式。

Use 64-bit Windows, the system .NET Framework 4.x compiler, and Windows PowerShell 5.1. Builds also work with PowerShell 7 on this machine; run form checks in Windows PowerShell with STA.

## 从源码构建 / Build from source

在项目根目录运行 / From the project root:

```powershell
& '.\scripts 脚本\构建 Build.ps1'
& '.\scripts 脚本\检查 Check.ps1'
```

无需现有 EXE、Python 或网络下载。输出在 `.build 本地构建/bin`，该目录已被 Git 忽略。

No existing EXE, Python, or downloads are needed. Output is written to `.build 本地构建/bin`, which is ignored by Git.

默认版本没有 FFmpeg 二进制。随机复制工具可直接使用；拼接工具依次选择：`PEAK_AUDIO_FFMPEG` 指定文件、内置组件、程序旁的 `ffmpeg.exe`。如果环境变量已设置但文件不存在，将明确报错。关闭程序只清理由它自己释放的临时组件，不会删除你的外置 FFmpeg。

Default builds contain no FFmpeg binary. The copier works directly. The joiner resolves FFmpeg in this order: `PEAK_AUDIO_FFMPEG`, embedded resource, then `ffmpeg.exe` beside the application. An invalid configured environment path produces an error. Cleanup only removes the application's own extracted temporary component, never your external FFmpeg.

```powershell
$env:PEAK_AUDIO_FFMPEG = 'D:\tools\ffmpeg.exe'
```

内置组件构建 / Build with an embedded component:

```powershell
& '.\scripts 脚本\构建 Build.ps1' -FFmpegPath 'D:\tools\ffmpeg.exe'
```

如本机脚本执行策略禁止运行，先按你自己的系统管理规则允许所需本地脚本；无需为这个项目永久修改机器策略。`检查 Check.ps1` 实例化窗口并验证中英文切换、文件命名、容量计算、复制内容和随机效果范围；不等于对所有播放器硬件或最终界面布局的全面验证。

If local script policy blocks execution, follow your own system administration rules for allowing local scripts; no permanent machine policy change is required by this project. Checks instantiate forms and validate languages, naming, capacity, copy bytes, and random ranges. They do not establish compatibility with all hardware players or complete visual layout coverage.

## 本地 ZIP 打包 / Prepare local ZIP files

不带音频与 FFmpeg 的源码构建工具包 / Source-built tools without game audio or FFmpeg:

```powershell
& '.\scripts 脚本\打包 Package.ps1' -Version '0.1.0'
```

带内置 FFmpeg 的工具包 / Tools with embedded FFmpeg:

```powershell
& '.\scripts 脚本\打包 Package.ps1' -FFmpegPath 'D:\tools\ffmpeg.exe' -Version '0.1.0-embedded'
```

从你现有资料包复制全部音频的完整包 / Full kit copying audio from your existing kit:

```powershell
& '.\scripts 脚本\打包 Package.ps1' `
  -FFmpegPath 'D:\tools\ffmpeg.exe' `
  -IncludeAudio `
  -KitPath 'D:\sharing\PEAK宾邦玩偶语音制作资料包 PEAK Bingbong Audio Plush Kit' `
  -Version '0.1.0-full'
```

`KitPath` 指向含 `02_音频素材 Audio Assets` 的文件夹。以上路径是示例，请替换为自己的位置。脚本创建新的临时目录来复制打包材料，不修改原始分享包。ZIP 与 `.sha256` 输出到 `releases 本地发布`；已有同名 ZIP 时停止，不覆盖。

`KitPath` must contain `02_音频素材 Audio Assets`. Replace example paths with your own. Packaging copies into a new temporary staging directory without changing the original kit. ZIP and `.sha256` files go into `releases 本地发布`. Existing archives are not overwritten.

## GitHub 上传 / Publish to GitHub

本项目使用独立 Git 仓库，仓库名为 `peak-bingbong-audio-tools`。如果下载的是源码 ZIP，请先在解压后的项目目录运行 `git init -b main`。以下步骤也可用于发布你自己的副本。

This project uses an independent Git repository named `peak-bingbong-audio-tools`. If you downloaded a source ZIP, first run `git init -b main` in the extracted project directory. The steps below can also be used to publish your own copy.

1. 在 GitHub 新建空仓库。/ Create an empty GitHub repository.
2. 只在这个项目目录中查看 `git status --short`，确认源码和文档。/ Review source and docs using `git status --short` inside this project.
3. 按你的身份配置 Git，提交后加入你创建的仓库地址。/ Configure your Git identity, commit, and add your repository URL.

```powershell
git add .
git commit -m 'Prepare bilingual Bingbong audio tools'
$remoteUrl = Read-Host 'Paste your GitHub repository URL'
git remote add origin $remoteUrl
git push -u origin main
```

若远程已存在初始提交，请按该仓库历史合并或用 GitHub Desktop 完成导入，不要直接强推。不要把父级游戏安装目录作为仓库根目录。

If the remote already has history, merge it appropriately or use GitHub Desktop. Do not force-push by default. The parent game installation directory is not the repository root.

4. 在 GitHub 的 Releases 页面创建发布，附上本地 ZIP、对应 `.sha256`，并复制发布说明。/ Create a Release, attach the chosen ZIP and `.sha256`, and copy the release notes.
5. 音频与内置 FFmpeg 的发布范围依照 [COPYRIGHT.md](../COPYRIGHT.md) 与 [LICENSING.md](../LICENSING.md) 确认；源码尚未指定开源许可证。/ Review audio provenance and component distribution materials; no project-wide source license has been selected.

GitHub 建议用 Releases 分发大型二进制；普通 Git 文件超过 50 MiB 会警告，超过 100 MiB 会被阻止，网页上传单文件上限为 25 MiB。此处的 ZIP 应作为 Release 附件，而不是上传到 Code 文件列表。[官方说明 / GitHub documentation](https://docs.github.com/en/repositories/working-with-files/managing-large-files/about-large-files-on-github)

## 自动构建 / CI

`.github/workflows/windows-build.yml` 配置了 push/PR/手动触发的 Windows 构建及行为检查，上传不含 FFmpeg 的工具构建产物。工作流仅有仓库内容读取权限，不会创建 Release 或上传游戏音频。GitHub 上的实际运行需在仓库上传后确认。

The Windows workflow builds and checks on push/PR/manual dispatch and uploads external-FFmpeg build artifacts. It uses read-only repository permissions and does not publish Releases or game audio. Its hosted execution must be confirmed after upload.
