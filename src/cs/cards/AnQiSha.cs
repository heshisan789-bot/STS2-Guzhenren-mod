using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class AnQiSha : AbstractShaZhaoCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    public AnQiSha()
        : base(1, CardType.Attack, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.JianDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        var previousAttackCount = CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.HappenedThisTurn(CombatState) &&
            e.CardPlay.Card.Owner == Owner &&
            e.CardPlay.Card.Type == CardType.Attack);
        var targetNotAttacking = !(cardPlay.Target.Monster?.IntendsToAttack ?? false);
        var damage = DynamicVars.Damage.BaseValue;
        if (previousAttackCount == 0 && targetNotAttacking)
        {
            damage *= 10m;
        }

        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
    }
}
