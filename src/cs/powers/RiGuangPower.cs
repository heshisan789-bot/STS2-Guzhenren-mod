using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Guzhenren.Scripts;

public sealed class RiGuangPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
}
