using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(CardModel), "get_Title")]
internal static class BenMingGuTitlePatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref string __result)
    {
        if (__instance is AbstractBenMingGuCard)
        {
            __result = __instance.TitleLocString.GetFormattedText();
        }
    }
}
