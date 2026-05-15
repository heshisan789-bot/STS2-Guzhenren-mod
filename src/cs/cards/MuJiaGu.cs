using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.MuDao)]
public sealed class MuJiaGu : AbstractGuZhenRenCard
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new PowerVar<FenShaoPower>(5m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FenShaoPower>()];

    public MuJiaGu() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        SetDao(GuZhenRenDao.MuDao);
        SetRank(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (!ReferenceEquals(card, this) || CombatState == null)
        {
            return;
        }

        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<FenShaoPower>(enemy, DynamicVars["FenShaoPower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["FenShaoPower"].UpgradeValueBy(2m);
        UpgradeRank(1);
    }
}
