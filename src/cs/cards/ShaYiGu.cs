using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class ShaYiGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new PowerVar<YiPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<YiPower>()];

    public ShaYiGu() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var enemyCount = CombatState.HittableEnemies.Count;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);

        if (enemyCount > 0)
        {
            await PowerCmd.Apply<YiPower>(Owner.Creature, enemyCount * DynamicVars["YiPower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        UpgradeRank(1);
    }
}
