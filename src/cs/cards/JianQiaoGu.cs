using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.JianDao)]
public sealed class JianQiaoGu : AbstractGuZhenRenCard
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<JianQiaoPower>()];

    public JianQiaoGu() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.JianDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        var selected = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1),
            context: choiceContext,
            player: Owner,
            filter: static c => c.Type == CardType.Attack,
            source: this)).FirstOrDefault();

        if (selected == null)
        {
            return;
        }

        selected.SetToFreeThisCombat();
        await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
        await PowerCmd.Apply<JianQiaoPower>(Owner.Creature, 1m, Owner.Creature, this);
        Owner.Creature.GetPower<JianQiaoPower>()?.AddStoredCard(selected);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        UpgradeRank(1);
    }
}
