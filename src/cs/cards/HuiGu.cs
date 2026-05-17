using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class HuiGu : AbstractGuZhenRenCard
{
    private const string CardsKey = "Cards";

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(CardsKey, 1m)];

    public HuiGu() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(7);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        var pickCount = Math.Max(0, (int)DynamicVars[CardsKey].BaseValue);
        if (pickCount == 0)
        {
            return;
        }

        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        var candidates = exhaustPile.Cards.ToList();
        if (candidates.Count == 0)
        {
            return;
        }

        List<CardModel> selected;
        if (candidates.Count <= pickCount)
        {
            selected = candidates;
        }
        else
        {
            var prompt = new LocString("cards", "GUZHENREN-HUI_GU.selectPrompt");
            selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, candidates, Owner, new CardSelectorPrefs(prompt, pickCount))).ToList();
        }

        foreach (var card in selected)
        {
            if (card.Pile?.Type != PileType.Exhaust)
            {
                continue;
            }

            if (card is HuiGu)
            {
                var regret = CombatState.CreateCard<Regret>(Owner);
                await CardPileCmd.AddGeneratedCardToCombat(regret, PileType.Hand, addedByPlayer: true);
            }

            await CardPileCmd.Add(card, PileType.Hand, source: this);
            card.SetToFreeThisTurn();
        }

        if (Owner.Character is not FangYuanCharacter)
        {
            var regret = CombatState.CreateCard<Regret>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(regret, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[CardsKey].UpgradeValueBy(1m);
        UpgradeRank(1);
    }
}
