using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

public sealed class YiPower : AbstractGuZhenRenPower
{
    private sealed class Data
    {
        public bool IsConverting;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await ResolveThresholds(applier ?? Owner, cardSource);
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && amount > 0)
        {
            await ResolveThresholds(applier ?? Owner, cardSource);
        }
    }

    private async Task ResolveThresholds(Creature applier, CardModel? cardSource)
    {
        var data = GetInternalData<Data>();
        if (data.IsConverting)
        {
            return;
        }

        data.IsConverting = true;
        try
        {
            ArgumentNullException.ThrowIfNull(Owner.Player, nameof(Owner.Player));

            while (Amount >= 3)
            {
                Flash();
                await PowerCmd.SetAmount<YiPower>(Owner, Amount - 3, applier, cardSource);
                await PlayerCmd.GainEnergy(1m, Owner.Player);
                await PowerCmd.Apply<QingPower>(Owner, 1, applier, cardSource);
                await ZhuanYiPower.TriggerConversion(Owner, applier, cardSource);
            }
        }
        finally
        {
            data.IsConverting = false;
        }
    }
}
