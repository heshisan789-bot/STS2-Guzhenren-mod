using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Guzhenren.Scripts;

public sealed class XingHuoLiaoYuanPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
}

