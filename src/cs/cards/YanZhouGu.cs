using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class YanZhouGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12, ValueProp.Move),
        new DynamicVar("Burns", 1m)
    ];

    public YanZhouGu() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        SetDao(GuZhenRenDao.YanDao);
        SetRank(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        for (var i = 0; i < DynamicVars["Burns"].IntValue; i++)
        {
            await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Burn>(Owner), PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
        UpgradeRank(1);
    }
}
