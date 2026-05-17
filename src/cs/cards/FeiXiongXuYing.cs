using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class FeiXiongXuYing : AbstractXuYingCard
{
    private const string StrengthMultiplierKey = "StrengthMultiplier";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..base.CanonicalVars,
        new DynamicVar(StrengthMultiplierKey, 2m),
        new CalculationBaseVar(5m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(static (CardModel card, Creature? _) =>
        {
            var multiplier = card.DynamicVars[StrengthMultiplierKey].BaseValue;
            var strength = card.Owner.Creature.GetPowerAmount<StrengthPower>();
            return strength * Math.Max(0m, multiplier - 1m);
        })
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public FeiXiongXuYing() : base(CardType.Attack, TargetType.AllEnemies, 25m)
    {
    }

    protected override async Task TriggerPhantomEffect(PlayerChoiceContext choiceContext, Creature? target)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[StrengthMultiplierKey].UpgradeValueBy(1m);
    }
}
