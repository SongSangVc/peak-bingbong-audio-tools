# 音频版本与来源 · Audio Sets and Provenance

本地已有素材共 **237 份 MP3、35 份 FSB**。统计包含原始文件、拼接版本及重复副本，不等于不同台词数量。源码仓库只提供清单；当前完整本地发布包保留了全部音频。

The existing local collection contains **237 MP3 and 35 FSB files**, including originals, joined versions, and repeated copies. These are not counts of unique voice lines. The source repository includes the manifest; the full local release kit retains the audio.

前置 `0.1 秒` 静音用于给发声器留出启动时间，减少音频开头漏播；可按模块调整。伪随机副本用于无随机模式的发声器，让预先打乱的文件按顺序播放时呈现近似随机的效果。

The `0.1-second` leading silence gives the sound module startup time to reduce lost openings; adjust it for the module. Pseudo-random copies provide a pre-shuffled sequence for sound modules without a random mode.

| 目录 / Folder | 文件数 / Files | MiB |
| --- | ---: | ---: |
| 01_原始语音 Original Voice Lines | 52 | 2.10 |
| 02_前奏及相关音效 Intros and Related Sounds | 18 | 1.13 |
| 03_拼接语音前置空白0.1秒 Joined Voices with 0.1s Lead | 26 | 1.20 |
| 04_拼接语音无前置空白 Joined Voices without Lead | 26 | 1.20 |
| 05_已有随机复制结果 Existing Random Copies | 150 | 4.00 |

- 原始语音：26 条 `bingbongVO`，各有 MP3 与 FSB。/ Original voices: 26 lines in MP3 and FSB.
- 相关音效：9 个，包含 `Au_BingBong_Squeak1`。/ Related sounds: 9 assets, including the chosen intro.
- 带前置空白版本：0.1 秒静音 → `Au_BingBong_Squeak1`，音量 1 倍 → 1 秒静音 → 语音。/ Leading version: 0.1 s silence, intro at 1x volume, 1 s gap, voice.
- 无前置空白版本：仅去掉最前面的 0.1 秒静音。/ No-lead version: only the initial silence was removed.
- 150 个历史随机复制文件保留旧编号，包含重复。/ Historical random copies retain old numbering and include repeats.

[完整文件清单 / Full manifest](音频文件清单%20Audio%20Manifest.csv) 只含分享包内相对路径，不含开发者本机绝对路径。

音频提取自 PEAK，权利归原权利人；调整和分享不构成授权。详见 [COPYRIGHT.md](../COPYRIGHT.md)。工具默认随机范围不代表游戏中实测参数。

Audio originates from PEAK and remains with its rights holders; modification and sharing do not grant permission. See [COPYRIGHT.md](../COPYRIGHT.md). Default random ranges are not measured in-game settings.
