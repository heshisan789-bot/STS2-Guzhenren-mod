using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

public sealed class ZhiZhangPower : AbstractGuZhenRenPower
{
    private sealed class Data
    {
        public decimal PendingHeal;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;
    public override int DisplayAmount => 0;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (target != Owner || canonicalPower is not NianPower || amount <= 0m)
        {
            return false;
        }

        GetInternalData<Data>().PendingHeal += amount;
        modifiedAmount = 0m;
        return true;
    }

    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        if (power is not NianPower)
        {
            return;
        }

        var data = GetInternalData<Data>();
        if (data.PendingHeal <= 0m)
        {
            return;
        }

        var healAmount = data.PendingHeal;
        data.PendingHeal = 0m;
        Flash();
        await CreatureCmd.Heal(Owner, healAmount);
    }
}
