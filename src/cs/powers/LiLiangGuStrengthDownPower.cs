using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

/// <summary>
/// 力量蛊的临时减力效果，对应塔一里的 LoseStrengthPower。
/// </summary>
public sealed class LiLiangGuStrengthDownPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<LiLiangGu>();

    protected override bool IsPositive => false;
}
