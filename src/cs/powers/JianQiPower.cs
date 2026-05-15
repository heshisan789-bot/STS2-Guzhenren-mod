using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guzhenren.Scripts;

public sealed class JianQiPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        var amount = Amount;
        await PowerCmd.Remove(this);
        if (amount > 0)
        {
            await PowerCmd.Apply<JianHenPower>(Owner, amount, Applier, null);
        }
    }
}
