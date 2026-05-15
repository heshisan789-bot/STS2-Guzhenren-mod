using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class FeiJian : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    public FeiJian() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.JianDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ResolveFlyingSword(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        UpgradeRank(1);
    }

    private async Task ResolveFlyingSword(PlayerChoiceContext choiceContext, Creature? initialTarget, decimal damage)
    {
        if (damage <= 0 || CombatState == null)
        {
            return;
        }

        var target = initialTarget;
        if (target == null || !target.IsAlive)
        {
            target = CombatState.HittableEnemies.FirstOrDefault();
        }

        if (target == null)
        {
            return;
        }

        await DamageCmd.Attack(damage).FromCard(this).Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var nextDamage = Math.Floor(damage / 2m);
        if (nextDamage > 0)
        {
            await ResolveFlyingSword(choiceContext, null, nextDamage);
        }
    }
}
