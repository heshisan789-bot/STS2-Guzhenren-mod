using System;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Guzhenren.Scripts;

public abstract class AbstractBenMingGuCard : AbstractGuZhenRenCard
{
    protected virtual int MaxRank => 9;

    public override int MaxUpgradeLevel => Math.Max(1, MaxRank - 1);

    protected AbstractBenMingGuCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target)
    {
    }

    public override bool IsBenMingGu => true;
}
