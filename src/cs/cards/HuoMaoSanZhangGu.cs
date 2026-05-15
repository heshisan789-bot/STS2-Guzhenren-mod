using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class HuoMaoSanZhangGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<HuoMaoSanZhangPower>()];

    public HuoMaoSanZhangGu() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        SetDao(GuZhenRenDao.YanDao);
        SetRank(4);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.GetPower<HuoMaoSanZhangPower>() == null)
        {
            await PowerCmd.Apply<HuoMaoSanZhangPower>(Owner.Creature, -1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        UpgradeRank(1);
    }
}
