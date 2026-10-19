# 许可范围 · Licensing Scope

## 项目源码与文档 / Project source and documentation

维护者尚未选择项目的开源许可证，因此没有在本仓库中擅自添加 MIT、GPL 或其他授权文本。公开源码不等同于授予这些许可证中的权利。需要超出现有允许范围的复用时，请联系维护者。今后如选择许可证，应明确其适用文件与范围。

The maintainer has not selected an open-source license. This repository therefore does not add an MIT, GPL, or other project-wide license grant. Publicly available source does not itself grant those permissions. Contact the maintainer for reuse beyond applicable permissions. Any future license should identify its scope.

## 游戏音频 / Game audio

游戏音频及其调整版本由原权利人保有权利，不包含在任何未来源码许可证的默认授权范围内。见 [COPYRIGHT.md](COPYRIGHT.md)。

Game audio and its modified versions retain their original ownership and are not automatically covered by any future source-code license. See [COPYRIGHT.md](COPYRIGHT.md).

## 第三方组件 / Third-party components

FFmpeg 的许可取决于实际构建配置。本地既有组件显示 `--enable-gpl --enable-version3`，其 `-L` 输出声明 GPL v3 或更新版本；不能将该二进制视为只有 imageio 包的 BSD 许可。原组件信息已保留。重新分发时应保留适用许可，并确认对应源码与其他义务；仅放上免责声明或上游链接不代表这些义务已经完成。

FFmpeg licensing depends on its build configuration. The existing local binary reports `--enable-gpl --enable-version3` and GPL v3-or-later terms in `-L`. The imageio wrapper's BSD notice is not the sole license for that binary. Original component information is retained. Redistributors should retain applicable notices and address corresponding-source and other requirements; a disclaimer or an upstream link alone does not establish compliance.

参考 / Reference: [FFmpeg licensing information](https://ffmpeg.org/legal.html).
