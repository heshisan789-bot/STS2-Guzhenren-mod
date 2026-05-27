using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhouDao)]
public sealed class ShiYin : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgraded ? [] : [CardKeyword.Ethereal];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IntangiblePower>()];

    protected override bool IsPlayable =>
        CombatState == null || CombatState.RoundNumber % 2 == 1;

    protected override bool ShouldGlowGoldInternal =>
        CombatState != null && CombatState.RoundNumber % 2 == 1;

    public ShiYin() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhouDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var creature = Owner.Creature;
        if (!creature.HasPower<IntangiblePower>())
        {
            await PowerCmd.Apply<IntangiblePower>(creature, 1m, creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        if (!IsCanonical)
        {
            RemoveKeyword(CardKeyword.Ethereal);
        }

        UpgradeRank(1);
    }
}
