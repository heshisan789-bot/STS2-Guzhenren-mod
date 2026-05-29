using Godot;
using HarmonyLib;
using MegaCrit.sts2.Core.Nodes.TopBar;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class FangYuanTopBarPortraitOverridePatch
{
    private const string FangYuanCharacterId = "GUZHENREN-FANG_YUAN_CHARACTER";
    private const string FangYuanCharacterIdLower = "guzhenren-fang_yuan_character";
    private const string FangYuanIconTexturePath = "res://guzhenren/images/character/FangYuan/Button.png";

    [HarmonyPatch(typeof(NTopBarPortrait), nameof(NTopBarPortrait.Initialize))]
    [HarmonyPostfix]
    private static void InitializePostfix(NTopBarPortrait __instance, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        var entry = player.Character.Id.Entry;
        if (!string.Equals(entry, FangYuanCharacterId, System.StringComparison.OrdinalIgnoreCase)
            && !string.Equals(entry, FangYuanCharacterIdLower, System.StringComparison.OrdinalIgnoreCase)
            && !(entry.Contains("guzhenren", System.StringComparison.OrdinalIgnoreCase)
                 && entry.Contains("fang_yuan", System.StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        var texture = ResourceLoader.Load<Texture2D>(FangYuanIconTexturePath, null, ResourceLoader.CacheMode.Reuse);
        if (texture == null)
        {
            return;
        }

        foreach (var child in __instance.GetChildren())
        {
            if (child is Node node)
            {
                node.QueueFree();
            }
        }

        var rect = new TextureRect { Texture = texture };
        rect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        rect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;

        rect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        rect.ZIndex = 999;
        rect.MouseFilter = Control.MouseFilterEnum.Ignore;

        __instance.AddChild(rect);
    }
}
