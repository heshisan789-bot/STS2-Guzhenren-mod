using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class GuZhenRenCardRankPatch
{
    private sealed class DescriptionCache
    {
        public string? Text { get; set; }
    }

    private static readonly ConditionalWeakTable<NCard, DescriptionCache> DescriptionCacheTable = new();

    private static readonly FieldInfo? DescriptionLabelField =
        typeof(NCard).GetField("_descriptionLabel", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly FieldInfo? PreviewTargetField =
        typeof(NCard).GetField("_previewTarget", BindingFlags.NonPublic | BindingFlags.Instance);

    private static IEnumerable<MethodBase> TargetMethods()
    {
        var updateVisuals = AccessTools.Method(typeof(NCard), "UpdateVisuals", [typeof(PileType), typeof(CardPreviewMode)]);
        if (updateVisuals != null)
        {
            yield return updateVisuals;
        }

        var patch1 = AccessTools.Method(typeof(NCard), "UpdateVisuals_Patch1");
        if (patch1 != null)
        {
            yield return patch1;
        }
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void UpdateVisualsPostfix(NCard __instance, object[] __args)
    {
        _ = __instance;
        _ = __args;
    }
}
