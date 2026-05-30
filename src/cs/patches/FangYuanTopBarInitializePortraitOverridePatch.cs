using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(NTopBar), nameof(NTopBar.Initialize))]
internal static class FangYuanTopBarInitializePortraitOverridePatch
{
    private const string FangYuanIconTexturePath = "res://guzhenren/images/character/FangYuan/Button.png";
    private static bool _loggedOnce;

    [HarmonyPostfix]
    private static void Postfix(NTopBar __instance, IRunState runState)
    {
        var player = LocalContext.GetMe(runState);
        var entry = player?.Character?.Id.Entry;
        if (string.IsNullOrWhiteSpace(entry))
        {
            return;
        }
        if (!(entry.Contains("guzhenren", System.StringComparison.OrdinalIgnoreCase)
              && entry.Contains("fang_yuan", System.StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        var texture = ResourceLoader.Load<Texture2D>(FangYuanIconTexturePath, null, ResourceLoader.CacheMode.Reuse);
        if (texture == null)
        {
            if (!_loggedOnce)
            {
                _loggedOnce = true;
                Log.Error($"[GuZhenRen][TopBar] texture load failed: entry={entry} path={FangYuanIconTexturePath}");
            }
            return;
        }

        var portrait = __instance.Portrait;
        if (portrait == null)
        {
            if (!_loggedOnce)
            {
                _loggedOnce = true;
                Log.Error($"[GuZhenRen][TopBar] portrait node is null: entry={entry}");
            }
            return;
        }

        if (!_loggedOnce)
        {
            _loggedOnce = true;
            Log.Info($"[GuZhenRen][TopBar] overriding portrait: entry={entry} size={portrait.Size} path={FangYuanIconTexturePath}");
        }

        foreach (var child in portrait.GetChildren())
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
        portrait.AddChild(rect);
    }
}
