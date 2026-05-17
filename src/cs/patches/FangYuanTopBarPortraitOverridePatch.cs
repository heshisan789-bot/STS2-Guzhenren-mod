using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.sts2.Core.Nodes.TopBar;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class FangYuanTopBarPortraitOverridePatch
{
    private const string FangYuanCharacterId = "GUZHENREN-FANG_YUAN_CHARACTER";
    private const string FangYuanCharacterIdLower = "guzhenren-fang_yuan_character";
    private const string FangYuanIconTexturePath = "res://guzhenren/images/character/FangYuan/Button.png";

    [HarmonyPatch(typeof(NTopBarPortrait), nameof(NTopBarPortrait.Initialize))]
    [HarmonyPrefix]
    private static bool InitializePrefix(NTopBarPortrait __instance, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        var entry = player.Character.Id.Entry;
        if (!string.Equals(entry, FangYuanCharacterId, System.StringComparison.OrdinalIgnoreCase)
            && !string.Equals(entry, FangYuanCharacterIdLower, System.StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!ResourceLoader.Exists(FangYuanIconTexturePath))
        {
            return true;
        }

        foreach (var child in __instance.GetChildren())
        {
            if (child is Node node)
            {
                node.QueueFree();
            }
        }

        var texture = PreloadManager.Cache.GetTexture2D(FangYuanIconTexturePath);
        var rect = new TextureRect
        {
            Texture = texture,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
        };

        rect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        rect.ZIndex = 999;
        rect.MouseFilter = Control.MouseFilterEnum.Ignore;

        __instance.AddChild(rect);
        return false;
    }
}
