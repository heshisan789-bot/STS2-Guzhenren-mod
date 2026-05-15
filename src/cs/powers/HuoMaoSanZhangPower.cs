using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

public sealed class HuoMaoSanZhangPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;
    public override int DisplayAmount => 0;

    public override decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (giver != Owner || amount <= 0m || power.Id != ModelDb.Power<FenShaoPower>().Id)
        {
            return amount;
        }

        return Math.Max(3m, amount);
    }
}
