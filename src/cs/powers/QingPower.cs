using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

public sealed class QingPower : AbstractGuZhenRenPower
{
    private const string ExtraDrawKey = "ExtraDraw";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(ExtraDrawKey, 0m)];

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        RefreshDescriptionVars();
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this)
        {
            RefreshDescriptionVars();
        }

        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var extraDraw = Amount / 3;
        if (extraDraw > 0)
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, extraDraw, player);
        }
    }

    private void RefreshDescriptionVars()
    {
        if (!IsMutable)
        {
            return;
        }

        DynamicVars[ExtraDrawKey].BaseValue = Amount / 3m;
    }
}
