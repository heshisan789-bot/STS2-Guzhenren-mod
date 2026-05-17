using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(PowerModel), "get_DumbHoverTip")]
internal static class GuZhenRenPowerTooltipPatch
{
    [HarmonyPrefix]
    private static bool Prefix(PowerModel __instance, ref HoverTip __result)
    {
        if (__instance.GetType().Assembly != typeof(Entry).Assembly)
        {
            return true;
        }

        var description = __instance.Description;
        description.Add("Amount", __instance.Amount);
        description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
        description.Add("energyPrefix", EnergyIconHelper.GetPrefix(__instance));
        __instance.DynamicVars.AddTo(description);

        __result = new HoverTip(__instance, description.GetFormattedText(), isSmart: false);
        return false;
    }
}

[HarmonyPatch(typeof(PowerModel), "get_HoverTips")]
internal static class GuZhenRenPowerHoverTipsPatch
{
    [HarmonyPostfix]
    private static void Postfix(PowerModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance.GetType().Assembly != typeof(Entry).Assembly)
        {
            return;
        }

        var list = (__result as IList<IHoverTip>) ?? __result.ToList();
        if (list.Count == 0)
        {
            return;
        }

        var description = (__instance.HasSmartDescription ? __instance.SmartDescription : __instance.Description);
        description.Add("Amount", __instance.Amount);
        description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
        description.Add("energyPrefix", EnergyIconHelper.GetPrefix(__instance));
        __instance.DynamicVars.AddTo(description);

        var isSmart = __instance.HasSmartDescription && __instance.IsMutable;
        list[0] = new HoverTip(__instance, description.GetFormattedText(), isSmart);
        __result = list;
    }
}
