using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guzhenren.Scripts;

/// <summary>
/// 蛊真人当前只在 NCard 完成默认渲染后强制替换能量 orb，
/// 先避免 STS2 默认图标覆盖，再单独继续寻找塔一卡框在塔二中的正确落点。
/// </summary>
[HarmonyPatch(typeof(NCard), "Reload")]
internal static class GuZhenRenCardAppearancePatch
{
    private static readonly FieldInfo? EnergyIconField =
        typeof(NCard).GetField("_energyIcon", BindingFlags.NonPublic | BindingFlags.Instance);

    [HarmonyPostfix]
    private static void ReloadPostfix(NCard __instance)
    {
        if (!GodotObject.IsInstanceValid(__instance) || __instance.Model is not AbstractGuZhenRenCard)
        {
            return;
        }

        if (EnergyIconField?.GetValue(__instance) is TextureRect energyIconRect)
        {
            if (!GodotObject.IsInstanceValid(energyIconRect))
            {
                return;
            }

            var energyOrbPath = GuZhenRenArtPaths.GetCardEnergyOrb(useBigOrb: false) ??
                                GuZhenRenArtPaths.GetCardEnergyOrb(useBigOrb: true);
            if (!string.IsNullOrEmpty(energyOrbPath))
            {
                var energyOrb = ResourceLoader.Load<Texture2D>(energyOrbPath);
                if (energyOrb != null)
                {
                    energyIconRect.Texture = energyOrb;
                }
            }

            ResetTextureRect(energyIconRect);
        }
    }

    private static void ResetTextureRect(CanvasItem item)
    {
        item.Material = null;
        item.Modulate = Colors.White;
        item.SelfModulate = Colors.White;
    }
}
