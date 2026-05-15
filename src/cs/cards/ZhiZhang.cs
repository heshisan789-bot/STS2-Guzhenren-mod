using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class ZhiZhang : AbstractGuZhenRenCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NianPower>(),
        HoverTipFactory.FromPower<ZhiZhangPower>()
    ];

    public ZhiZhang() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var nianPower = Owner.Creature.GetPower<NianPower>();
        if (nianPower != null)
        {
            await PowerCmd.Remove(nianPower);
        }

        await PowerCmd.Apply<ZhiZhangPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        UpgradeRank(1);
    }
}
