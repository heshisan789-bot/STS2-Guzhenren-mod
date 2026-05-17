using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guzhenren.Scripts;

public sealed class SheXinDrawReductionPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player || AmountOnTurnStart == 0 || Amount <= 0)
        {
            return count;
        }

        return Math.Max(0m, count - Amount);
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == Owner.Side && AmountOnTurnStart != 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}
