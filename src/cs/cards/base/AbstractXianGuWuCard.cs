using MegaCrit.Sts2.Core.Entities.Cards;

namespace Guzhenren.Scripts;

public abstract class AbstractXianGuWuCard : AbstractShaZhaoCard
{
    protected AbstractXianGuWuCard(int cost, CardType type, TargetType target)
        : base(cost, type, target)
    {
    }

    public override bool IsXianGu => true;
}
