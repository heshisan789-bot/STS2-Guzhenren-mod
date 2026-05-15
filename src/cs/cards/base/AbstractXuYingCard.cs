using MegaCrit.Sts2.Core.Entities.Cards;

namespace Guzhenren.Scripts;

public abstract class AbstractXuYingCard : AbstractGuZhenRenCard
{
    protected AbstractXuYingCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target)
    {
    }

    public override bool IsXuYing => true;
}
