using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(PowerModel), nameof(PowerModel.Icon), MethodType.Getter)]
internal static class GuZhenRenPowerPackedIconPatch
{
    private static bool Prefix(PowerModel __instance, ref Texture2D __result)
    {
        if (!__instance.Id.Entry.StartsWith("GUZHENREN-", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var path = GuZhenRenArtPaths.GetPowerPackedIcon(__instance.GetType().Name);
        if (path == null)
        {
            return true;
        }

        __result = PreloadManager.Cache.GetTexture2D(path);
        return false;
    }
}
