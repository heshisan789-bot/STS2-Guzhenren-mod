using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Multiplayer.Game;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(RestSiteSynchronizer), "ChooseOption")]
internal static class ShaZhaoCraftRestSiteNoConsumePatch
{
    private sealed class Cache
    {
        public bool WasCraft { get; set; }
        public List<RestSiteOption>? Snapshot { get; set; }
    }

    private static readonly ConditionalWeakTable<Player, Cache> CacheTable = new();

    [HarmonyPrefix]
    private static void ChooseOptionPrefix(RestSiteSynchronizer __instance, Player player, int optionIndex)
    {
        var options = __instance.GetOptionsForPlayer(player);
        var wasCraft = optionIndex >= 0 && optionIndex < options.Count && options[optionIndex] is ShaZhaoCraftRestSiteOption;
        var cache = CacheTable.GetOrCreateValue(player);
        cache.WasCraft = wasCraft;
        cache.Snapshot = wasCraft ? options.ToList() : null;
    }

    [HarmonyPostfix]
    private static void ChooseOptionPostfix(RestSiteSynchronizer __instance, Player player, ref Task<bool> __result)
    {
        __result = PostProcess(__instance, player, __result);
    }

    private static async Task<bool> PostProcess(RestSiteSynchronizer synchronizer, Player player, Task<bool> resultTask)
    {
        var success = await resultTask;
        if (!success)
        {
            return false;
        }

        if (!CacheTable.TryGetValue(player, out var cache) || !cache.WasCraft || cache.Snapshot == null)
        {
            return true;
        }

        if (synchronizer.GetOptionsForPlayer(player) is not List<RestSiteOption> options)
        {
            return true;
        }

        options.Clear();
        options.AddRange(cache.Snapshot);

        return true;
    }
}
