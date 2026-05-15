using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class HuoGu : AbstractBenMingGuCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new PowerVar<FenShaoPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FenShaoPower>()];

    public HuoGu() : base(1, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.YanDao);
        SetRank(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var handPile = PileType.Hand.GetPile(Owner);
        var maxSelect = Math.Min(DynamicVars.Cards.IntValue, handPile.Cards.Count);
        if (maxSelect <= 0)
        {
            return;
        }

        var selected = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 0, maxSelect),
            context: choiceContext,
            player: Owner,
            filter: static _ => true,
            source: this);

        var exhaustedCount = 0;
        foreach (var card in selected)
        {
            await CardPileCmd.Add(card, PileType.Exhaust);
            exhaustedCount++;
        }

        for (var i = 0; i < exhaustedCount; i++)
        {
            await PowerCmd.Apply<FenShaoPower>(cardPlay.Target, DynamicVars["FenShaoPower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnRankChanged()
    {
        base.OnRankChanged();

        var clampedRank = Math.Clamp(Rank, 1, 9);
        var consumes = new[] { 3m, 3m, 4m, 5m, 6m, 7m, 8m, 8m, 9m };
        DynamicVars.Cards.BaseValue = consumes[clampedRank - 1];

        if (IsCanonical)
        {
            return;
        }

        if (clampedRank == 1)
        {
            AddKeyword(CardKeyword.Ethereal);
        }
        else
        {
            RemoveKeyword(CardKeyword.Ethereal);
        }

        if (clampedRank >= 8)
        {
            AddKeyword(CardKeyword.Retain);
        }
        else
        {
            RemoveKeyword(CardKeyword.Retain);
        }
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }
}
