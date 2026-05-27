using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Runs;

namespace Guzhenren.Scripts;

public static class GuFangStartingRecipeHelper
{
    public static async Task TryGrantStartingRecipe(RunState runState)
    {
        foreach (var player in runState.Players)
        {
            if (player.Character is not FangYuanCharacter)
            {
                continue;
            }

            if (player.Relics.OfType<AbstractGuFangRecipeRelic>().Any())
            {
                continue;
            }

            var seed = $"{runState.Rng.StringSeed}|{player.NetId}|GUZHENREN_START_RECIPE";
            var idx = (int)((uint)StringHelper.GetDeterministicHashCode(seed) % 15u);
            await ObtainByIndex(player, idx);
        }
    }

    private static Task ObtainByIndex(Player player, int idx)
    {
        return idx switch
        {
            0 => RelicCmd.Obtain<RecipeYangMangBeiHuoYi>(player),
            1 => RelicCmd.Obtain<RecipeWuZhiQuanXinJian>(player),
            2 => RelicCmd.Obtain<RecipeXuePiaoLiu>(player),
            3 => RelicCmd.Obtain<RecipeXueJianLeng>(player),
            4 => RelicCmd.Obtain<RecipeZhuMoBang>(player),
            5 => RelicCmd.Obtain<RecipeTianPuGuangHe>(player),
            6 => RelicCmd.Obtain<RecipeWuJinXuanGuangQi>(player),
            7 => RelicCmd.Obtain<RecipeWanWoDaShouYin>(player),
            8 => RelicCmd.Obtain<RecipeAnQiSha>(player),
            9 => RelicCmd.Obtain<RecipeSanShiSanTianGuang>(player),
            10 => RelicCmd.Obtain<RecipeJianLangSanDie>(player),
            11 => RelicCmd.Obtain<RecipeJianHenSuoMing>(player),
            12 => RelicCmd.Obtain<RecipeAngryBird>(player),
            13 => RelicCmd.Obtain<RecipeWanXingFeiYing>(player),
            _ => RelicCmd.Obtain<RecipeXingXiuQiPan>(player)
        };
    }
}
