using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class DaoChiXueFu : AbstractGuZhenRenCard
{
    private const string CalculatedHitsKey = "CalculatedHits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedHitsKey).WithMultiplier(static (card, _) => ((DaoChiXueFu)card).CalculateHits())
    ];

    public DaoChiXueFu() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var hits = CalculateHits();
        for (var i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_bloody_impact")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        UpgradeRank(1);
    }

    private int CalculateHits()
    {
        return CountCopiesIn(PileType.Hand)
            + CountCopiesIn(PileType.Draw)
            + CountCopiesIn(PileType.Discard)
            + CountCopiesIn(PileType.Exhaust)
            + CountCopiesIn(PileType.Play);
    }

    private int CountCopiesIn(PileType pileType)
    {
        return pileType.GetPile(Owner).Cards.Count(card => card is DaoChiXueFu);
    }
}
