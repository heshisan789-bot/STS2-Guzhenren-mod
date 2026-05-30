using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

/// <summary>
/// 万物大同变：将获得的非道痕类 Power 全部转化为变化道道痕。
/// </summary>
public sealed class WanWuDaTongBianPower : AbstractGuZhenRenPower
{
    private sealed class Data
    {
        public bool IsConverting;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    protected override object InitInternalData() => new Data();

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await ConvertExistingPowers(applier, cardSource);
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount <= 0)
            return;

        await ConvertPower(power, amount, applier, cardSource);
    }

    private async Task ConvertExistingPowers(Creature? applier, CardModel? cardSource)
    {
        var data = GetInternalData<Data>();
        if (data.IsConverting)
            return;

        data.IsConverting = true;
        try
        {
            var powersToConvert = Owner.Powers
                .Where(p => p != this && !IsProtectedPower(p) && p.Amount > 0)
                .ToList();

            foreach (var power in powersToConvert)
            {
                var amt = (int)power.Amount;
                // 移除被转化的 Power（排除特殊 Power 的部分层数保护逻辑简化处理）
                await PowerCmd.Remove(power);
                await PowerCmd.Apply<BianHuaDaoDaoHenPower>(Owner, amt, applier ?? Owner, cardSource);
            }
        }
        finally
        {
            data.IsConverting = false;
        }
    }

    private async Task ConvertPower(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (IsProtectedPower(power))
            return;

        var data = GetInternalData<Data>();
        if (data.IsConverting)
            return;

        data.IsConverting = true;
        try
        {
            Flash();
            var amt = (int)amount;
            await PowerCmd.Remove(power);
            await PowerCmd.Apply<BianHuaDaoDaoHenPower>(Owner, amt, applier ?? Owner, cardSource);
        }
        finally
        {
            data.IsConverting = false;
        }
    }

    /// <summary>
    /// 不应被转化的受保护 Power 类型。
    /// </summary>
    private static bool IsProtectedPower(PowerModel power) =>
        power is BianHuaDaoDaoHenPower ||
        power is WanWuDaTongBianPower;
}
