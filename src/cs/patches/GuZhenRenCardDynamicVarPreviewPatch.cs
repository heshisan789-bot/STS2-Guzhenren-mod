using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.UpdateDynamicVarPreview))]
internal static class GuZhenRenCardDynamicVarPreviewPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel __instance, CardPreviewMode previewMode, Creature? target, DynamicVarSet dynamicVarSet)
    {
        if (__instance is not AbstractGuZhenRenCard)
        {
            return true;
        }

        if (__instance.CombatState == null)
        {
            return true;
        }

        foreach (var v in dynamicVarSet.Values.ToList())
        {
            v.UpdateCardPreview(__instance, previewMode, target, runGlobalHooks: true);
        }

        return false;
    }
}
