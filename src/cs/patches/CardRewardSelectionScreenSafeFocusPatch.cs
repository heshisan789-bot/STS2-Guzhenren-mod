using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class CardRewardSelectionScreenSafeFocusPatch
{
    [HarmonyPatch(typeof(NCardRewardSelectionScreen), "get_DefaultFocusedControl")]
    [HarmonyPrefix]
    private static bool GetDefaultFocusedControlPrefix(NCardRewardSelectionScreen __instance, ref Control __result)
    {
        var lastFocused = Traverse.Create(__instance).Field<Control>("_lastFocusedControl").Value;
        if (lastFocused != null)
        {
            __result = lastFocused;
            return false;
        }

        var cardRow = Traverse.Create(__instance).Field<Control>("_cardRow").Value;
        if (cardRow == null)
        {
            __result = __instance;
            return false;
        }

        var holders = cardRow.GetChildren().OfType<Control>().ToList();
        if (holders.Count == 0)
        {
            __result = cardRow;
            return false;
        }

        __result = holders[holders.Count / 2];
        return false;
    }
}

