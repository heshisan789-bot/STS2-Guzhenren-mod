using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class GuZhenRenNCardForcePreviewPatch
{
    private static readonly AccessTools.FieldRef<NCard, bool> ForceUnpoweredPreviewRef =
        AccessTools.FieldRefAccess<NCard, bool>("_forceUnpoweredPreview");

    [HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
    [HarmonyPrefix]
    private static void UpdateVisualsPrefix(NCard __instance, ref bool __state)
    {
        __state = false;
        if (__instance.Model is not AbstractGuZhenRenCard)
        {
            return;
        }

        ref var forceUnpoweredPreview = ref ForceUnpoweredPreviewRef(__instance);
        __state = forceUnpoweredPreview;
        if (forceUnpoweredPreview)
        {
            forceUnpoweredPreview = false;
        }
    }

    [HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
    [HarmonyPostfix]
    private static void UpdateVisualsPostfix(NCard __instance, bool __state)
    {
        if (!__state)
        {
            return;
        }

        if (__instance.Model is not AbstractGuZhenRenCard)
        {
            return;
        }

        ref var forceUnpoweredPreview = ref ForceUnpoweredPreviewRef(__instance);
        forceUnpoweredPreview = true;
    }
}
