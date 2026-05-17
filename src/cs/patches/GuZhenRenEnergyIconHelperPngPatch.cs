using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;

namespace Guzhenren.Scripts;

[HarmonyPatch]
internal static class GuZhenRenEnergyIconHelperPngPatch
{
    private const string GuZhenRenPrefix = "guzhenren";
    private const string GuZhenRenEnergyTexturePath = "res://images/packed/sprite_fonts/guzhenren_energy_icon.png";

    [HarmonyPatch(typeof(EnergyIconHelper), nameof(EnergyIconHelper.GetPath), typeof(string))]
    [HarmonyPostfix]
    private static void GetPathPostfix(string prefix, ref string __result)
    {
        if (string.Equals(prefix, GuZhenRenPrefix, System.StringComparison.OrdinalIgnoreCase))
        {
            __result = GuZhenRenEnergyTexturePath;
        }
    }
}

