# Guzhenren Demo

当前仓库已经具备以下 demo 组成：

- `Guzhenren.dll`：C# 逻辑程序集
- `Guzhenren.json`：模组清单
- `guzhenren/`：本地图片、场景、文本资源
- `export_presets.cfg`：Godot 导出 `Guzhenren.pck` 的预设
- `scripts/package_demo.sh`：一键打包脚本

## 打包 demo

先确保本机已安装：

- Slay the Spire 2
- Godot 4.5+
- BaseLib 3.1.3
- （可选）RitsuLib：当前 demo 不依赖 RitsuLib

如果你下载的是 `Godot 4.5.1 mono`，macOS 上通常安装成 `/Applications/Godot_mono.app`，脚本会自动识别；也可以手动指定：

```bash
GODOT_BIN="/Applications/Godot_mono.app/Contents/MacOS/Godot" ./scripts/package_demo.sh
```

然后在仓库根目录执行：

```bash
./scripts/package_demo.sh
```

默认会输出到：

```text
dist/Guzhenren/
```

里面应当包含：

- `Guzhenren.dll`
- `Guzhenren.pck`
- `Guzhenren.json`

如果想让脚本直接把文件安装到游戏的 `mods` 目录（避免手动复制），可以：

```bash
INSTALL_DIR="/Users/ryan/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods" ./scripts/package_demo.sh
```

## 当前 demo 内容

- 角色卡图、能力图标、药水图、遗物图已切到本地资源
- 方源已接入战斗立绘场景
- 方源已接入角色小图标场景
- 方源已接入能量球场景
- 方源已接入休息点场景
- 方源已接入商店场景
- 方源已接入选角背景场景

## 已知缺口

- 当前机器若未安装 Godot 编辑器，则无法在本仓库内直接产出 `.pck`
- 目前角色演示场景是静态贴图版，不是最终 Spine 成品
- 仍有部分原版机制与卡牌尚未迁完，当前更接近“可进入并试玩的 demo”，不是完整正式版
