using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

public sealed class FeiLiPower : AbstractGuZhenRenPower
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

        Flash();
        var artifact = Owner.GetPower<ArtifactPower>();
        if (artifact != null)
        {
            await PowerCmd.Decrement(artifact);
        }
        else
        {
            await PowerCmd.Apply<StrengthPower>(Owner, -1m, Applier, null);
        }

        if (Amount <= 1)
        {
            await PowerCmd.Remove(this);
        }
        else
        {
            await PowerCmd.SetAmount<FeiLiPower>(Owner, Amount - 1, Applier, null);
        }
    }
}

