using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.GuangDao)]
public sealed class JuGuangGu : AbstractGuZhenRenCard
{
    private const string CalculatedShanYaoKey = "CalculatedShanYao";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ShanYaoPower>(1m),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedShanYaoKey).WithMultiplier(static (card, _) => ((JuGuangGu)card).CalculateShanYaoAmount())
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShanYaoPower>()];

    public JuGuangGu() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.GuangDao);
        SetRank(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var amount = CalculateShanYaoAmount();
        if (amount > 0)
        {
            ShanYaoPower.RecordGain(amount);
            await PowerCmd.Apply<ShanYaoPower>(Owner.Creature, amount, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ShanYaoPower"].UpgradeValueBy(1m);
        UpgradeRank(1);
    }

    private decimal CalculateShanYaoAmount()
    {
        return CountGuangDaoCardsInHand() * DynamicVars["ShanYaoPower"].BaseValue;
    }

    private int CountGuangDaoCardsInHand()
    {
        return PileType.Hand.GetPile(Owner).Cards.Count(card => card is AbstractGuZhenRenCard guCard && guCard.PrimaryDao == GuZhenRenDao.GuangDao);
    }
}
