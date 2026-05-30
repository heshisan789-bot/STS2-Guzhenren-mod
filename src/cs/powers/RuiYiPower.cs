using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Guzhenren.Scripts;

public sealed class RuiYiPower : AbstractGuZhenRenPower
{
    public static bool IsActive { get; set; }

    static RuiYiPower()
    {
        BattleStateManager.OnBattleStart(() => IsActive = false);
        BattleStateManager.OnPostBattle(() => IsActive = false);
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;
    public override int DisplayAmount => 0;
}
