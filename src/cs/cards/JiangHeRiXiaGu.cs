using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.GuangDao)]
public sealed class JiangHeRiXiaGu : AbstractGuZhenRenCard
{
    private const string CalculatedHitsKey = "CalculatedHits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedHitsKey).WithMultiplier(static (card, _) => ((JiangHeRiXiaGu)card).CalculateHitsForDisplay())
    ];

    public JiangHeRiXiaGu() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.GuangDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var hits = CalculateHitsForPlay();
        for (var i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        UpgradeRank(1);
    }

    private int CalculateHitsForDisplay()
    {
        return CountGuangDaoCardsInHand();
    }

    private int CalculateHitsForPlay()
    {
        var handCount = CountGuangDaoCardsInHand();
        return Pile?.Type == PileType.Hand ? handCount : handCount + 1;
    }

    private int CountGuangDaoCardsInHand()
    {
        return PileType.Hand.GetPile(Owner).Cards.Count(card => card is AbstractGuZhenRenCard guCard && guCard.PrimaryDao == GuZhenRenDao.GuangDao);
    }
}
