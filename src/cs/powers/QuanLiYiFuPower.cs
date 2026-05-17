using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Guzhenren.Scripts;

public sealed class QuanLiYiFuPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;
    public override int DisplayAmount => 0;
}

