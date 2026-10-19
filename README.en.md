# PEAK Bingbong Audio Plush Tools · 宾邦玩偶语音工具

**English** · [中文](README.md)

Maintained by **Song_Sang** (SongSang).

Prepare audio for a DIY talking Bingbong plush: add intros, adjust effects, and generate a pseudo-random set of playback files within a storage budget.

This is a personal fan project. The entire project was completed with assistance from **OpenAI Codex**, including audio extraction and processing, tool development, and documentation. The applications target **64-bit Windows with .NET Framework 4.x**, with Chinese and English interfaces.

## Why these features exist

**Make sequential playback feel approximately random.** Some plush sound modules have no random playback mode. The copier prepares a longer shuffled, repeated sequence within a storage budget, so sequential-only devices can sound more varied. The hardware remains sequential; its actual ordering may depend on numbering, filenames, or write order.

**Allow time for the sound module to start.** The default `0.1-second` silence before the intro reduces the chance of losing the audio's opening during startup. Test and adjust it for your module: increase it if the start is cut off, or shorten it, including to `0`, for faster modules. Leading silence and the intro/voice gap are separate settings.

## Applications

| Tool | Features |
| --- | --- |
| Random Audio Copier | Calculates output count from the capacity limit and source sizes; shuffles and repeats full rounds; names files `1_original-name.mp3`, `2_original-name.mp3`; preserves audio bytes. |
| Intro Voice Joiner | Uses one intro file or random intros from a folder; sets leading silence and the intro/voice gap; applies optional volume, pitch, and ranged random effects per segment; exports MP3/WAV/FLAC. |

Fixed seeds reproduce ordering and effects. Pitch changes also affect speed and duration. The default random range, `0.90–1.10`, is an adjustable starting point, not a measured in-game setting.

## Download and use

[Download the tools or full kit from GitHub Releases](https://github.com/Song-Siyu-05/peak-bingbong-audio-tools/releases/latest). The repository now includes the curated audio. GitHub source ZIPs include it, but not built EXEs.

1. Download a published tools ZIP or full kit and extract it completely.
2. Run an EXE in `01_实用程序 Applications`.
3. Select English using `Language / 语言` at the top.
4. Try a small audio set and capacity first, then transfer the results to your player.

The full kit's EXEs can be moved individually. A joiner **built with embedded FFmpeg** needs no separate audio component. The default source build uses an external FFmpeg instead.

- [Bilingual User Guide · 使用指导](docs%20文档/使用指导%20User%20Guide.txt)
- [Audio sets and provenance](docs%20文档/音频素材说明%20Audio%20Assets.md)

## Build locally

From the project root, preferably in **PowerShell 7** (`pwsh`):

```powershell
& '.\scripts 脚本\构建 Build.ps1'
```

Output is written to `.build 本地构建/bin`. The copier works directly. For the joiner, put `ffmpeg.exe` beside the EXE or set `PEAK_AUDIO_FFMPEG` to its full path.

To embed an FFmpeg executable you supply:

```powershell
& '.\scripts 脚本\构建 Build.ps1' -FFmpegPath 'D:\tools\ffmpeg.exe'
pwsh -NoProfile -STA -File '.\scripts 脚本\检查 Check.ps1'
```

No previous EXE is required and nothing is downloaded automatically. See [Build and Release](docs%20文档/构建与发布%20Build%20and%20Release.md) for checks and packaging.

## Repository layout

- `src 源码/`: both C# WinForms applications and language resources.
- `audio 音频/`: extracted originals and adjusted audio sets.
- `scripts 脚本/`: build, behavior checks, and local release packaging.
- `docs 文档/`: user guides, asset manifest, and publication instructions.
- `third-party 第三方组件/`: existing component notices.
- `.github/`: feedback templates and Windows build workflow.
- `releases 本地发布/`: locally prepared release attachments, ignored by Git.

The [audio directory](audio%20音频/) includes extracted MP3/FSB originals, both joined versions, and existing random copies. Game installations, FFmpeg binaries, and future copies generated elsewhere remain excluded from Git.

## Contact and rights

Email: **SongSang_Yichang@outlook.com**. I only check this inbox occasionally; replies may be delayed and are not guaranteed. See [Contact](CONTACT.md).

Bingbong voice lines and related effects were extracted from **PEAK** and remain the property of their rights holders. This fan project claims no official affiliation, authorization, or endorsement. Extraction and modification do not change ownership; a disclaimer does not grant permission. See [Copyright](COPYRIGHT.md).

No open-source license has been selected for the project code. Public source does not imply an MIT/GPL license grant. FFmpeg and other third-party components retain their own licensing terms. See [Licensing](LICENSING.md) and [Third-party notices](third-party%20第三方组件/README.md).
