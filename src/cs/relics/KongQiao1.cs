using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Guzhenren.Scripts;

public abstract class AbstractKongQiaoRelic : CustomRelicModel
{
    private const string RemainingXpKey = "RemainingXp";

    private bool _effectUsedThisCombat;

    private int _xp;

    public abstract int Rank { get; }
    protected abstract int NeededXp { get; }
    protected abstract string RelicImageName { get; }
    protected abstract RelicModel? NextStage { get; }

    public override RelicRarity Rarity => Rank == 1 ? RelicRarity.Starter : RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(RemainingXpKey, 0m)];

    [SavedProperty]
    public int Xp
    {
        get => _xp;
        set
        {
            AssertMutable();
            _xp = Math.Max(0, value);
            RefreshDescriptionVars();
        }
    }

    public override string PackedIconPath => GuZhenRenArtPaths.GetRelicIcon(RelicImageName);
    protected override string PackedIconOutlinePath => GuZhenRenArtPaths.GetRelicOutline(RelicImageName);
    protected override string BigIconPath => GuZhenRenArtPaths.GetRelicIcon(RelicImageName);

    public override Task BeforeCombatStart()
    {
        _effectUsedThisCombat = false;
        return Task.CompletedTask;
    }

    public override async Task BeforeCombatStartLate()
    {
        if (Rank >= 6 && Owner.Creature.CombatState != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(CreateImmortalEssenceCard(), PileType.Hand, addedByPlayer: true);
        }
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_effectUsedThisCombat || cardPlay.IsAutoPlay || !QualifiesForKongQiao(cardPlay.Card))
        {
            return Task.CompletedTask;
        }

        _effectUsedThisCombat = true;
        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (!ShouldMakeFreeInCombat(card))
        {
            return false;
        }

        modifiedCost = 0m;
        return true;
    }

    public override bool TryModifyStarCost(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (!ShouldMakeFreeInCombat(card))
        {
            return false;
        }

        modifiedCost = 0m;
        return true;
    }

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (NextStage == null)
        {
            return;
        }

        GainXp(GetXpReward(room.RoomType));
        if (Xp < NeededXp)
        {
            return;
        }

        var overflowXp = Xp - NeededXp;
        var nextStage = NextStage.ToMutable();
        if (nextStage is not AbstractKongQiaoRelic nextKongQiao)
        {
            throw new InvalidOperationException($"Next kong qiao stage for {GetType().Name} must also inherit {nameof(AbstractKongQiaoRelic)}.");
        }

        nextKongQiao.Xp = overflowXp;
        await RelicCmd.Replace(this, nextKongQiao);
        AutoUpgradeBenMingGu(nextKongQiao.Rank);
    }

    protected bool QualifiesForKongQiao(CardModel card)
    {
        return Rank > 1 &&
               card.Owner == Owner &&
               card is AbstractGuZhenRenCard guCard &&
               guCard.Rank >= 1 &&
               guCard.Rank < Rank;
    }

    private bool ShouldMakeFreeInCombat(CardModel card)
    {
        if (_effectUsedThisCombat || !QualifiesForKongQiao(card))
        {
            return false;
        }

        return card.Pile?.Type is PileType.Hand or PileType.Play;
    }

    private CardModel CreateImmortalEssenceCard()
    {
        ArgumentNullException.ThrowIfNull(Owner.Creature.CombatState, nameof(Owner.Creature.CombatState));

        return Rank switch
        {
            6 => Owner.Creature.CombatState.CreateCard<QingTiXianYuan>(Owner),
            7 => Owner.Creature.CombatState.CreateCard<HongZaoXianYuan>(Owner),
            8 => Owner.Creature.CombatState.CreateCard<BaiLiXianYuan>(Owner),
            9 => Owner.Creature.CombatState.CreateCard<HuangXingXianYuan>(Owner),
            _ => Owner.Creature.CombatState.CreateCard<BaiLiXianYuan>(Owner)
        };
    }

    private void GainXp(int amount)
    {
        if (amount <= 0 || NextStage == null)
        {
            return;
        }

        Xp += amount;
    }

    private int GetXpReward(RoomType roomType)
    {
        return roomType switch
        {
            RoomType.Boss => 5,
            RoomType.Elite => 3,
            RoomType.Monster => 1,
            _ => 0
        };
    }

    private void AutoUpgradeBenMingGu(int targetRank)
    {
        foreach (var card in Owner.Deck.Cards.OfType<AbstractBenMingGuCard>())
        {
            while (card.Rank < targetRank && card.IsUpgradable)
            {
                CardCmd.Upgrade(card);
            }
        }
    }

    private void RefreshDescriptionVars()
    {
        if (!IsMutable)
        {
            return;
        }

        DynamicVars[RemainingXpKey].BaseValue = Math.Max(0, NeededXp - Xp);
    }
}

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class KongQiao1 : AbstractKongQiaoRelic
{
    public override int Rank => 1;
    protected override int NeededXp => 1;
    protected override string RelicImageName => "KongQiao_1";
    protected override RelicModel? NextStage => ModelDb.Relic<KongQiao2>();
}

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class KongQiao2 : AbstractKongQiaoRelic
{
    public override int Rank => 2;
    protected override int NeededXp => 2;
    protected override string RelicImageName => "KongQiao_2";
    protected override RelicModel? NextStage => ModelDb.Relic<KongQiao3>();
}

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class KongQiao3 : AbstractKongQiaoRelic
{
    public override int Rank => 3;
    protected override int NeededXp => 3;
    protected override string RelicImageName => "KongQiao_3";
    protected override RelicModel? NextStage => ModelDb.Relic<KongQiao4>();
}

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class KongQiao4 : AbstractKongQiaoRelic
{
    public override int Rank => 4;
    protected override int NeededXp => 4;
    protected override string RelicImageName => "KongQiao_4";
    protected override RelicModel? NextStage => ModelDb.Relic<KongQiao5>();
}

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class KongQiao5 : AbstractKongQiaoRelic
{
    public override int Rank => 5;
    protected override int NeededXp => 5;
    protected override string RelicImageName => "KongQiao_5";
    protected override RelicModel? NextStage => ModelDb.Relic<XianQiao6>();
}

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class XianQiao6 : AbstractKongQiaoRelic
{
    public override int Rank => 6;
    protected override int NeededXp => 6;
    protected override string RelicImageName => "XianQiao_6";
    protected override RelicModel? NextStage => ModelDb.Relic<XianQiao7>();
}

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class XianQiao7 : AbstractKongQiaoRelic
{
    public override int Rank => 7;
    protected override int NeededXp => 7;
    protected override string RelicImageName => "XianQiao_7";
    protected override RelicModel? NextStage => ModelDb.Relic<XianQiao8>();
}

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class XianQiao8 : AbstractKongQiaoRelic
{
    public override int Rank => 8;
    protected override int NeededXp => 8;
    protected override string RelicImageName => "XianQiao_8";
    protected override RelicModel? NextStage => ModelDb.Relic<XianQiao9>();
}

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class XianQiao9 : AbstractKongQiaoRelic
{
    public override int Rank => 9;
    protected override int NeededXp => 999;
    protected override string RelicImageName => "XianQiao_9";
    protected override RelicModel? NextStage => null;
}
