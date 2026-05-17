using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class YunSuan : AbstractGuZhenRenCard
{
    private const string CardsKey = "Cards";
    private const string IncreaseKey = "Increase";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(CardsKey, 2m),
        new DynamicVar(IncreaseKey, 10m)
    ];

    public YunSuan() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var handCards = PileType.Hand.GetPile(Owner).Cards.ToList();
        var exhaustCount = Math.Min(2, handCards.Count);
        if (exhaustCount > 0)
        {
            var selected = exhaustCount == handCards.Count
                ? handCards
                : (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, exhaustCount), null, this)).ToList();

            foreach (var card in selected)
            {
                await CardCmd.Exhaust(choiceContext, card);
            }
        }

        await CardPileCmd.Draw(choiceContext, DynamicVars[CardsKey].BaseValue, Owner);

        var delta = DynamicVars[IncreaseKey].BaseValue / 100m;
        if (delta <= 0m)
        {
            return;
        }

        foreach (var card in PileType.Hand.GetPile(Owner).Cards)
        {
            if (card is IGuZhenRenProbabilityCard probabilityCard)
            {
                probabilityCard.IncreaseBaseChance(delta);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[IncreaseKey].UpgradeValueBy(5m);
        UpgradeRank(1);
    }
}
