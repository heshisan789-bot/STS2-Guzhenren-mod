using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class ZhuanYiPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public static async Task TriggerConversion(Creature owner, Creature? applier, CardModel? cardSource)
    {
        var power = owner.GetPower<ZhuanYiPower>();
        if (power == null || power.Amount <= 0)
        {
            return;
        }

        power.Flash();
        await CreatureCmd.GainBlock(owner, power.Amount, ValueProp.Unpowered, null);
    }
}
