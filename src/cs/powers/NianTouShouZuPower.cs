using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

public sealed class NianTouShouZuPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;
    public override int DisplayAmount => 0;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (target != Owner || canonicalPower is not NianPower)
        {
            return false;
        }

        if (Owner.HasPower<ZhiZhangPower>())
        {
            return false;
        }

        if (amount <= 0m)
        {
            return true;
        }

        Flash();
        modifiedAmount = 0m;
        return true;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        return ClearNianIfNeeded();
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this)
        {
            return ClearNianIfNeeded();
        }

        return Task.CompletedTask;
    }

    private Task ClearNianIfNeeded()
    {
        if (Owner.HasPower<ZhiZhangPower>())
        {
            return Task.CompletedTask;
        }

        var nian = Owner.GetPower<NianPower>();
        return nian == null ? Task.CompletedTask : PowerCmd.Remove(nian);
    }
}
