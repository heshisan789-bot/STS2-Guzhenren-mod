using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Multiplayer.Game;

namespace Guzhenren.Scripts;

[HarmonyPatch(typeof(RestSiteSynchronizer), "ChooseOption")]
internal static class XianGuCanHaiSmithRestSitePatch
{
    private sealed class ChoiceCache
    {
        public bool WasSmith { get; set; }
    }

    private static readonly ConditionalWeakTable<Player, ChoiceCache> Cache = new();

    [HarmonyPrefix]
    private static void ChooseOptionPrefix(RestSiteSynchronizer __instance, Player player, int optionIndex)
    {
        var options = __instance.GetOptionsForPlayer(player);
        var wasSmith = optionIndex >= 0 && optionIndex < options.Count && options[optionIndex] is SmithRestSiteOption;
        Cache.GetOrCreateValue(player).WasSmith = wasSmith;
    }

    [HarmonyPostfix]
    private static void ChooseOptionPostfix(RestSiteSynchronizer __instance, Player player, int optionIndex, ref Task<bool> __result)
    {
        __result = PostProcess(__instance, player, optionIndex, __result);
    }

    private static async Task<bool> PostProcess(RestSiteSynchronizer synchronizer, Player player, int optionIndex, Task<bool> resultTask)
    {
        var success = await resultTask;
        if (!success)
        {
            return false;
        }

        if (!Cache.TryGetValue(player, out var cache) || !cache.WasSmith)
        {
            return true;
        }

        var relic = player.GetRelic<XianGuCanHai>();
        if (relic == null || relic.Counter <= 0)
        {
            return true;
        }

        relic.Counter -= 1;
        relic.Flash();

        if (synchronizer.GetOptionsForPlayer(player) is not List<RestSiteOption> options)
        {
            return true;
        }

        options.Add(new SmithRestSiteOption(player));
        return true;
    }
}
