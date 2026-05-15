using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class XueHeMang : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(14m),
        new ExtraDamageVar(4m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(static (card, _) => ((XueHeMang)card).CountHpLossEventsThisCombat())
    ];

    public XueHeMang() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(4m);
        DynamicVars.ExtraDamage.UpgradeValueBy(2m);
        UpgradeRank(1);
    }

    private int CountHpLossEventsThisCombat()
    {
        if (!CombatManager.Instance.IsInProgress)
        {
            return 0;
        }

        return CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>()
            .Count(entry => entry.Receiver == Owner.Creature && entry.Result.UnblockedDamage > 0m);
    }
}
