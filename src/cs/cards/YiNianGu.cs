using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class YiNianGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new PowerVar<NianPower>(3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NianPower>()];

    public YiNianGu() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var discardPile = PileType.Discard.GetPile(Owner);
        if (discardPile.Cards.Count > 0)
        {
            var maxSelect = Math.Min(DynamicVars.Cards.IntValue, discardPile.Cards.Count);
            IEnumerable<CardModel> selected = maxSelect >= discardPile.Cards.Count
                ? discardPile.Cards.ToList()
                : new List<CardModel>(await CardSelectCmd.FromSimpleGrid(choiceContext, discardPile.Cards, Owner, new CardSelectorPrefs(SelectionScreenPrompt, maxSelect)));

            foreach (var card in selected)
            {
                await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
            }
        }

        await PowerCmd.Apply<NianPower>(Owner.Creature, DynamicVars["NianPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NianPower"].UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}
