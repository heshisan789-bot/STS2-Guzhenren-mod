# GuZhenRen Resource Layout

当前角色仍使用 `PlaceholderCharacterModel`，所以这版骨架先不强依赖自定义场景与贴图。
后续补正式资源时，建议按下面结构放置：

- `guzhenren/images/cards/`：卡图
- `guzhenren/images/relics/`：遗物图
- `guzhenren/images/powers/`：能力图标
- `guzhenren/images/character_select/`：选角图标
- `guzhenren/scenes/character/`：方源战斗/休息/商店场景
- `guzhenren/scenes/ui/`：能量球与角色图标场景
- `guzhenren/localization/zhs/`：中文本地化

等资源准备好后，再把 `FangYuanCharacter` 从占位资源切到正式路径。
