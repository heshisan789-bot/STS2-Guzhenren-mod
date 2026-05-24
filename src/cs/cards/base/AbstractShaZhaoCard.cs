using MegaCrit.Sts2.Core.Entities.Cards;

namespace Guzhenren.Scripts;

public abstract class AbstractShaZhaoCard : AbstractGuZhenRenCard
{
    protected AbstractShaZhaoCard(int cost, CardType type, TargetType target)
        : base(cost, type, CardRarity.Token, target)
    {
        SetRank(0);
    }

    public override bool IsShaZhao => true;

    public override bool CanBeGeneratedInCombat => false;

    public override bool CanBeGeneratedByModifiers => false;
}
