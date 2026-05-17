# Guzhenren（蛊真人 / 方源）- 杀戮尖塔2 角色 MOD

[![Godot 4.5](https://img.shields.io/badge/Godot-4.5-blue.svg)](https://godotengine.org/)
[![C#](https://img.shields.io/badge/Language-C%23-green.svg)](https://learn.microsoft.com/dotnet/csharp/)
[![STS2](https://img.shields.io/badge/Game-Slay%20the%20Spire%202-red.svg)](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/)

## 简介

本项目为《杀戮尖塔2》(Slay the Spire 2) 开发的自定义角色 MOD，目标是把塔一 Java 版《蛊真人》MOD（方源）迁移到塔二（Godot 4.5 + C#/.NET）。

## 依赖

- BaseLib（本工程使用的版本随仓库内 `other-lib/BaseLib.*` 对照，但运行时以游戏 mods 目录里的 BaseLib 为准）

## 特性（阶段性）

- 单一角色卡池 `GuZhenRenCardPool`，内部按“道”拆分逻辑子池（便于后续局内筛选/屏蔽某些道）

## 项目结构

```
src/cs/                      # C# 脚本（卡牌/遗物/Power/patch/工具等）
guzhenren/
  scenes/                    # Godot 场景（人物、UI等）
  images/                    # 图片资源（卡图/遗物/能力图标等）
  localization/              # 本地化（zhs/en）
Guzhenren.json               # MOD 清单（manifest）
export_presets.cfg           # Godot 导出 pck 预设
scripts/package_demo.sh      # 一键构建/导出/打包（可选安装到游戏 mods）
other-lib/                   # 对照塔一规格与参考工程（运行时不依赖）
```

## 构建与打包

```bash
./scripts/package_demo.sh
```

输出默认在：

```text
dist/Guzhenren/
```

包含：

- `Guzhenren.dll`
- `Guzhenren.pck`
- `Guzhenren.json`

可选：直接安装到游戏 mods 目录：

```bash
INSTALL_DIR="/path/to/SlayTheSpire2.app/Contents/MacOS/mods" ./scripts/package_demo.sh
```

## 常见坑

- 本地化必须在 `res://<manifest.id>/localization/<locale>/*.json`，不要覆盖原版的本地化目录
- 关键词重复：STS2 会自动注入卡牌关键词文本；本地化里不要再手写“消耗/虚无/保留/不可打出”等关键词行
- 选牌 API：`FromChooseACardScreen` 只适合少量选项，多选项优先 `FromSimpleGrid`

## 致谢

- Mega Crit《杀戮尖塔2》
- BaseLib-StS2 项目与社区
- 塔一原项目[GuZhenRen 项目](https://github.com/PetStzx/StS_Mod_GuZhenRen)
