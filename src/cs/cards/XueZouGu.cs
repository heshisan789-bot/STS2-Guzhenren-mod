using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class XueZouGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(2m),
        new CardsVar(2)
    ];

    public XueZouGu() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);

        if (IsUpgraded)
        {
            await PowerCmd.Apply<DrawCardsNextTurnPower>(Owner.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
    }
}
