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
public sealed class BaShan : AbstractGuZhenRenCard
{
    private const string StrengthMultiplierKey = "StrengthMultiplier";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(StrengthMultiplierKey, 3m),
        new CalculationBaseVar(15m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>
        {
            var multiplier = card.DynamicVars[StrengthMultiplierKey].BaseValue;
            var strength = card.Owner.Creature.GetPowerAmount<StrengthPower>();
            return strength * Math.Max(0m, multiplier - 1m);
        })
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public BaShan() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.LiDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
        DynamicVars[StrengthMultiplierKey].UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}
