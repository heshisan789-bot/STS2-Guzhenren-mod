using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class WoLiXuYing : AbstractXuYingCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars;

    private CardModel? _cachedAttack;

    public WoLiXuYing() : base(CardType.Attack, TargetType.AnyEnemy, 15m)
    {
    }

    protected override async Task TriggerPhantomEffect(PlayerChoiceContext choiceContext, Creature? target)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        _cachedAttack ??= RollRandomAttackFromDeck();
        if (_cachedAttack == null)
        {
            return;
        }

        var canonical = ModelDb.GetById<CardModel>(_cachedAttack.Id);
        var generated = CombatState.CreateCard(canonical, Owner);
        for (var i = 0; i < _cachedAttack.CurrentUpgradeLevel; i++)
        {
            generated.UpgradeInternal();
            generated.FinalizeUpgradeInternal();
        }

        var added = await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, addedByPlayer: false);
        if (!added.success || added.cardAdded.Pile?.Type != PileType.Hand)
        {
            _cachedAttack = RollRandomAttackFromDeck();
            return;
        }
        var dupe = generated.CreateDupe();
        await CardPileCmd.RemoveFromCombat(generated, skipVisuals: true);

        var dupeAdded = await CardPileCmd.AddGeneratedCardToCombat(dupe, PileType.Hand, addedByPlayer: false);
        if (!dupeAdded.success || dupeAdded.cardAdded.Pile?.Type != PileType.Hand)
        {
            _cachedAttack = RollRandomAttackFromDeck();
            return;
        }
        dupe.SetToFreeThisTurn();
        await CardCmd.AutoPlay(choiceContext, dupe, target);

        _cachedAttack = RollRandomAttackFromDeck();
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ChanceKey].UpgradeValueBy(10m);
    }

    private CardModel? RollRandomAttackFromDeck()
    {
        var candidates = Owner.Deck.Cards
            .Where(card => card.Type == CardType.Attack)
            .Where(card => card is not AbstractXuYingCard)
            .Where(card => !card.Keywords.Contains(CardKeyword.Exhaust))
            .ToList();

        return Owner.RunState.Rng.Shuffle.NextItem(candidates);
    }
}
