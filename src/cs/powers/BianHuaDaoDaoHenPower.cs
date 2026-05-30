using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

/// <summary>
/// 变化道道痕：打出带有某流派标签的卡牌时，将此道痕转化为对应流派的道痕。
/// 转化时触发转意获得格挡。
/// </summary>
public sealed class BianHuaDaoDaoHenPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is not AbstractGuZhenRenCard guCard)
            return;

        var dao = guCard.PrimaryDao;
        if (dao == GuZhenRenDao.None || dao == GuZhenRenDao.BianHuaDao)
            return;

        var currentAmount = (int)Amount;

        switch (dao)
        {
            case GuZhenRenDao.LiDao:
                Flash();
                await PowerCmd.Remove(this);
                await PowerCmd.Apply<LiQiPower>(Owner, currentAmount, Owner, cardPlay.Card);
                await ZhuanYiPower.TriggerConversion(Owner, Owner, cardPlay.Card);
                break;
            case GuZhenRenDao.YanDao:
                Flash();
                await PowerCmd.Remove(this);
                await PowerCmd.Apply<FenShaoPower>(Owner, currentAmount, Owner, cardPlay.Card);
                await ZhuanYiPower.TriggerConversion(Owner, Owner, cardPlay.Card);
                break;
            case GuZhenRenDao.JianDao:
                Flash();
                await PowerCmd.Remove(this);
                await PowerCmd.Apply<JianFengPower>(Owner, currentAmount, Owner, cardPlay.Card);
                await ZhuanYiPower.TriggerConversion(Owner, Owner, cardPlay.Card);
                break;
            case GuZhenRenDao.XueDao:
                Flash();
                await PowerCmd.Remove(this);
                await PowerCmd.Apply<XueZhanPower>(Owner, currentAmount, Owner, cardPlay.Card);
                await ZhuanYiPower.TriggerConversion(Owner, Owner, cardPlay.Card);
                break;
            case GuZhenRenDao.ZhiDao:
                Flash();
                await PowerCmd.Remove(this);
                await PowerCmd.Apply<NianPower>(Owner, currentAmount, Owner, cardPlay.Card);
                await ZhuanYiPower.TriggerConversion(Owner, Owner, cardPlay.Card);
                break;
            case GuZhenRenDao.GuangDao:
                Flash();
                await PowerCmd.Remove(this);
                await PowerCmd.Apply<ShanYaoPower>(Owner, currentAmount, Owner, cardPlay.Card);
                await ZhuanYiPower.TriggerConversion(Owner, Owner, cardPlay.Card);
                break;
            case GuZhenRenDao.FengDao:
                Flash();
                await PowerCmd.Remove(this);
                await PowerCmd.Apply<LiQiPower>(Owner, currentAmount, Owner, cardPlay.Card);
                await ZhuanYiPower.TriggerConversion(Owner, Owner, cardPlay.Card);
                break;
        }
    }
}
