using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class MaLiXuYing : AbstractXuYingCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => base.CanonicalVars;

    public MaLiXuYing() : base(CardType.Skill, TargetType.None, 20m)
    {
    }

    protected override async Task TriggerPhantomEffect(PlayerChoiceContext choiceContext, Creature? target)
    {
        await CardPileCmd.Draw(choiceContext, 1, Owner);

        if (PileType.Hand.GetPile(Owner).Cards.Count == 0)
        {
            return;
        }

        var selected = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1),
            context: choiceContext,
            player: Owner,
            filter: static _ => true,
            source: this)).ToList();

        if (selected.Count > 0)
        {
            await CardCmd.Discard(choiceContext, selected);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[ChanceKey].UpgradeValueBy(15m);
    }
}
