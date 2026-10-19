# v0.1.1 · 原始音频与 PowerShell 7 / Original Audio and PowerShell 7

## 内容 / Included

- 两个中英文 Windows 工具：随机复制、前奏语音拼接。/ Two bilingual Windows tools: random copying and intro/voice joining.
- 容量上限自动计算数量，文件名为“编号_原文件名”。/ Capacity-based counts and index_original-name filenames.
- 前奏文件或目录、静音、音量与音高范围随机。/ Intro file/folder, silence, volume, and ranged pitch/volume randomization.
- 中英文使用指南、源码、构建说明和联系声明。/ Chinese and English guides, source, build instructions, and contact notices.

## 附件选择 / Choosing an asset

- 文件名含 `Tools`：工具、源码、文档，不含游戏音频。/ Tools, source, docs; no game audio.
- 文件名含 `Full Kit`：另含当前 237 份 MP3、35 份 FSB。/ Also includes the current 237 MP3 and 35 FSB files.
- `external`：拼接工具需自行配置 FFmpeg；`embedded`：包含构建时指定的 FFmpeg。/ External builds need a separately configured FFmpeg; embedded builds include the selected component.
- `.sha256`：ZIP 的 SHA256 校验值。/ ZIP checksum.

完整包中包含多个处理版本和重复复制结果，不代表 237 条不同台词。请完整解压后运行；音频模块兼容性按实际硬件确认。所有资料来源及组件许可见随包文档。

The full kit includes multiple versions and repeated copies, not 237 distinct lines. Extract completely before running; check compatibility with your actual playback hardware. See the supplied provenance and component notices.

反馈 / Contact: SongSang_Yichang@outlook.com（偶尔查看 / checked occasionally）。整个项目使用 OpenAI Codex 辅助完成。/ The entire project was completed with assistance from OpenAI Codex.

项目发布页 / Project releases: https://github.com/SongSangVc/peak-bingbong-audio-tools/releases

指南已合并为一份中英双语文件，并解释了顺序发声器的伪随机播放用途与前置静音的启动缓冲作用。 / The guide is now one bilingual file, explaining pre-shuffled sequences for sequential sound modules and leading silence for startup time.

## 本次更新 / This update

提取的原始 MP3 / FSB 及调整后的音频已直接放进仓库，可从 `audio 音频/` 浏览下载。构建与界面检查优先使用 PowerShell 7；GitHub 工作流通过 STA 运行窗口检查。

Extracted original MP3/FSB and prepared audio are now directly available in `audio 音频/`. Builds and form checks prefer PowerShell 7; the GitHub workflow uses STA for form checks.
