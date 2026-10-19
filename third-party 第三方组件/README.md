# 第三方组件 · Third-party Components

拼接工具通过独立进程调用 FFmpeg，不修改其二进制。构建时可选嵌入组件；默认源码构建不带 FFmpeg。

The joiner invokes FFmpeg as a separate process without modifying its binary. Embedding is optional; the default source build contains no FFmpeg executable.

现有本地资料包使用从 `imageio-ffmpeg 0.6.0` Windows wheel 得到的 `ffmpeg-win-x86_64-v7.1.exe`。具体构建参数与许可见同目录的原始文本；imageio 的包许可与 FFmpeg 二进制许可是不同事项。

The existing local kit uses `ffmpeg-win-x86_64-v7.1.exe` from the `imageio-ffmpeg 0.6.0` Windows wheel. Its build configuration and license output are retained here. The wrapper package license and the FFmpeg executable license are distinct.

- [FFmpeg 许可与构建信息](音频组件许可%20FFmpeg%20License.txt)
- [imageio-ffmpeg 包许可](打包组件许可%20Imageio%20FFmpeg%20License.txt)
- [FFmpeg official licensing information](https://ffmpeg.org/legal.html)
- [imageio-ffmpeg upstream](https://github.com/imageio/imageio-ffmpeg)

本仓库不声称已提供该预编译 FFmpeg 及其全部依赖的对应源码。上传含内置组件的发布包前，应确认其分发材料；也可提供外置组件模式的工具包，让使用者自行配置 FFmpeg。

This repository does not claim to provide the corresponding source for that prebuilt FFmpeg and all dependencies. Review distribution materials before publishing an embedded-component release. External-component builds allow users to configure FFmpeg themselves.
