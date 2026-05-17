using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.GuangDao)]
public sealed class SanShiSanTianGuang : AbstractShaZhaoCard
{
    private const string CalculatedShanYaoGainKey = "CalculatedShanYaoGain";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar(CalculatedShanYaoGainKey).WithMultiplier(static (_, _) => ShanYaoPower.TotalGainedThisCombat)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShanYaoPower>()];

    public SanShiSanTianGuang() : base(2, CardType.Attack, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.GuangDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var amount = ShanYaoPower.TotalGainedThisCombat;
        if (amount > 0)
        {
            ShanYaoPower.RecordGain(amount);
            await PowerCmd.Apply<ShanYaoPower>(Owner.Creature, amount, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
