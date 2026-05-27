using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System;

namespace Guzhenren.Scripts;

public abstract class AbstractXuYingCard : AbstractGuZhenRenCard, IGuZhenRenProbabilityCard
{
    protected const string ChanceKey = "Chance";

    private readonly decimal _baseChancePercent;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Unplayable];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(ChanceKey, _baseChancePercent)];

    protected AbstractXuYingCard(CardType type, TargetType target, decimal baseChancePercent)
        : base(0, type, CardRarity.Token, target)
    {
        _baseChancePercent = baseChancePercent;
        SetDao(GuZhenRenDao.LiDao);
        SetRank(0);
    }

    public override bool IsXuYing => true;

    public override GuZhenRenBannedCardSources BannedSources => GuZhenRenBannedCardSources.All;

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Pile?.Type != PileType.Hand || cardPlay.Card.Owner != Owner)
        {
            return;
        }

        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card is AbstractXuYingCard || cardPlay.Card.IsDupe)
        {
            return;
        }

        var chance = (float)GetEffectiveChance();
        if (chance <= 0f || Owner.RunState.Rng.Shuffle.NextFloat() > chance)
        {
            Owner.Creature.GetPower<ZhuanYunPower>()?.OnProbabilityRollFailed(this);
            return;
        }

        await TriggerPhantom(choiceContext, ResolveTarget(cardPlay.Target));
    }

    public Task TriggerPhantom(PlayerChoiceContext choiceContext, Creature? preferredTarget)
    {
        return TriggerPhantomEffect(choiceContext, ResolveTarget(preferredTarget));
    }

    protected abstract Task TriggerPhantomEffect(PlayerChoiceContext choiceContext, Creature? target);

    private decimal GetEffectiveChance()
    {
        if (Owner.Creature.HasPower<QuanLiYiFuPower>() && CombatState != null && Pile is { Type: not PileType.Deck })
        {
            return 1m;
        }

        return DynamicVars[ChanceKey].BaseValue / 100m;
    }

    public void IncreaseBaseChance(decimal deltaProbability)
    {
        if (deltaProbability == 0m)
        {
            return;
        }

        var deltaPercent = deltaProbability * 100m;
        DynamicVars[ChanceKey].BaseValue = Math.Clamp(DynamicVars[ChanceKey].BaseValue + deltaPercent, 0m, 100m);
    }

    private Creature? ResolveTarget(Creature? preferredTarget)
    {
        if (TargetType is TargetType.None or TargetType.Self or TargetType.AllEnemies)
        {
            return preferredTarget;
        }

        if (preferredTarget is { IsAlive: true } && preferredTarget.CombatState == CombatState)
        {
            return preferredTarget;
        }

        var combatState = CombatState;
        if (combatState == null)
        {
            return preferredTarget;
        }

        return Owner.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
    }
}
