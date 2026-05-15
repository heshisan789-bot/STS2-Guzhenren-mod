using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class JinGangNian : AbstractGuZhenRenCard
{
    protected override bool HasEnergyCostX => true;

    protected override bool ShouldGlowGoldInternal => CalculateNianGainedThisTurn() > 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(6m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(static (card, _) => ((JinGangNian)card).CalculateNianGainedThisTurn())
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NianPower>()];

    public JinGangNian() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        var target = cardPlay.Target;

        var hitCount = ResolveEnergyXValue();
        if (IsUpgraded)
        {
            hitCount += 1;
        }

        if (hitCount <= 0)
        {
            return;
        }

        await DamageCmd.Attack(DynamicVars.CalculatedDamage).WithHitCount(hitCount).FromCard(this)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }

    private int CalculateNianGainedThisTurn()
    {
        if (CombatState == null)
        {
            return 0;
        }

        decimal total = 0m;
        foreach (var entry in CombatManager.Instance.History.Entries.OfType<PowerReceivedEntry>())
        {
            if (!entry.HappenedThisTurn(CombatState) || entry.Amount <= 0m)
            {
                continue;
            }

            if (entry.Power is NianPower && entry.Power.Owner == Owner.Creature)
            {
                total += entry.Amount;
            }
        }

        return (int)total;
    }
}
