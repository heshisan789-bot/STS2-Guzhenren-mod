using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace Guzhenren.Scripts;

public sealed class GuZhenRenShaZhaoSynthesisSystem : AbstractModel
{
    public override bool ShouldReceiveCombatHooks => false;

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (room?.RoomType != RoomType.Elite)
        {
            return false;
        }

        if (player.Character is not FangYuanCharacter)
        {
            return false;
        }

        var recipeRelic = CreateEliteRecipeRelic(player);
        rewards.Add(new RelicReward(recipeRelic, player));
        return true;
    }

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player.Character is not FangYuanCharacter)
        {
            return false;
        }

        options.Add(new ShaZhaoCraftRestSiteOption(player));
        return true;
    }

    private static RelicModel CreateEliteRecipeRelic(Player player)
    {
        var candidates = new List<int>(15);
        for (var i = 0; i < 15; i++)
        {
            var id = GetRecipeRelicIdByIndex(i);
            if (!player.Relics.Any(r => r.Id == id))
            {
                candidates.Add(i);
            }
        }

        if (candidates.Count == 0)
        {
            for (var i = 0; i < 15; i++)
            {
                candidates.Add(i);
            }
        }

        var runState = player.RunState;
        var seed = $"{runState.Rng.StringSeed}|{player.NetId}|{runState.TotalFloor}|{runState.CurrentRoomCount}|GUZHENREN_ELITE_RECIPE";
        var pickIndex = (int)((uint)StringHelper.GetDeterministicHashCode(seed) % (uint)candidates.Count);
        return CreateRecipeRelicByIndex(candidates[pickIndex]);
    }

    private static ModelId GetRecipeRelicIdByIndex(int idx)
    {
        return idx switch
        {
            0 => ModelDb.Relic<RecipeYangMangBeiHuoYi>().Id,
            1 => ModelDb.Relic<RecipeWuZhiQuanXinJian>().Id,
            2 => ModelDb.Relic<RecipeXuePiaoLiu>().Id,
            3 => ModelDb.Relic<RecipeXueJianLeng>().Id,
            4 => ModelDb.Relic<RecipeZhuMoBang>().Id,
            5 => ModelDb.Relic<RecipeTianPuGuangHe>().Id,
            6 => ModelDb.Relic<RecipeWuJinXuanGuangQi>().Id,
            7 => ModelDb.Relic<RecipeWanWoDaShouYin>().Id,
            8 => ModelDb.Relic<RecipeAnQiSha>().Id,
            9 => ModelDb.Relic<RecipeSanShiSanTianGuang>().Id,
            10 => ModelDb.Relic<RecipeJianLangSanDie>().Id,
            11 => ModelDb.Relic<RecipeJianHenSuoMing>().Id,
            12 => ModelDb.Relic<RecipeAngryBird>().Id,
            13 => ModelDb.Relic<RecipeWanXingFeiYing>().Id,
            _ => ModelDb.Relic<RecipeXingXiuQiPan>().Id
        };
    }

    private static RelicModel CreateRecipeRelicByIndex(int idx)
    {
        return idx switch
        {
            0 => ModelDb.Relic<RecipeYangMangBeiHuoYi>().ToMutable(),
            1 => ModelDb.Relic<RecipeWuZhiQuanXinJian>().ToMutable(),
            2 => ModelDb.Relic<RecipeXuePiaoLiu>().ToMutable(),
            3 => ModelDb.Relic<RecipeXueJianLeng>().ToMutable(),
            4 => ModelDb.Relic<RecipeZhuMoBang>().ToMutable(),
            5 => ModelDb.Relic<RecipeTianPuGuangHe>().ToMutable(),
            6 => ModelDb.Relic<RecipeWuJinXuanGuangQi>().ToMutable(),
            7 => ModelDb.Relic<RecipeWanWoDaShouYin>().ToMutable(),
            8 => ModelDb.Relic<RecipeAnQiSha>().ToMutable(),
            9 => ModelDb.Relic<RecipeSanShiSanTianGuang>().ToMutable(),
            10 => ModelDb.Relic<RecipeJianLangSanDie>().ToMutable(),
            11 => ModelDb.Relic<RecipeJianHenSuoMing>().ToMutable(),
            12 => ModelDb.Relic<RecipeAngryBird>().ToMutable(),
            13 => ModelDb.Relic<RecipeWanXingFeiYing>().ToMutable(),
            _ => ModelDb.Relic<RecipeXingXiuQiPan>().ToMutable()
        };
    }
}
