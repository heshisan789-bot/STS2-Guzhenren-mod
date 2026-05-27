using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class JianYing : AbstractGuZhenRenCard
{
    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal, CardKeyword.Unplayable];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];

    public JianYing()
        : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.JianDao);
        SetRank(0);
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (!ReferenceEquals(card, this) || CombatState == null)
        {
            return;
        }

        var enemies = CombatState.HittableEnemies.ToList();
        if (enemies.Count == 0)
        {
            return;
        }

        var maxStacks = enemies.Max(e => e.GetPowerAmount<JianHenPower>());
        var candidates = enemies.Where(e => e.GetPowerAmount<JianHenPower>() == maxStacks).ToList();
        var target = Owner.RunState.Rng.CombatTargets.NextItem(candidates);
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
