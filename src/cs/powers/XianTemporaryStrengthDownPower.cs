using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

public sealed class XianTemporaryStrengthDownPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Xian>();

    protected override bool IsPositive => false;
}
