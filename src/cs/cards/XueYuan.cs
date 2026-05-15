using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.XueDao)]
public sealed class XueYuan : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<XueYuanMarkPower>(2m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<XueYuanPower>(),
        HoverTipFactory.FromPower<XueYuanMarkPower>()
    ];

    public XueYuan() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.XueDao);
        SetRank(7);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var playerHasImmunity = Owner.Creature.Powers.Any(power => power.Id.Entry == "WAN_WU_DA_TONG_BIAN_POWER");
        if (playerHasImmunity)
        {
            await RemovePlayerBond();
            return;
        }

        var targetHasArtifact = cardPlay.Target.GetPowerAmount<ArtifactPower>() > 0;
        if (targetHasArtifact)
        {
            await PowerCmd.Apply<XueYuanMarkPower>(cardPlay.Target, DynamicVars["XueYuanMarkPower"].BaseValue, Owner.Creature, this);
            return;
        }

        if (Owner.Creature.GetPower<XueYuanPower>() == null)
        {
            await PowerCmd.Apply<XueYuanPower>(Owner.Creature, -1m, Owner.Creature, this);
        }

        await PowerCmd.Apply<XueYuanMarkPower>(cardPlay.Target, DynamicVars["XueYuanMarkPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["XueYuanMarkPower"].UpgradeValueBy(1m);
        UpgradeRank(1);
    }

    private async Task RemovePlayerBond()
    {
        var existingBond = Owner.Creature.GetPower<XueYuanPower>();
        if (existingBond != null)
        {
            await PowerCmd.Remove(existingBond);
        }
    }
}
