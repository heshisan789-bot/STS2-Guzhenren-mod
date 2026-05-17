using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

public sealed class LiLiangGuStrengthDownPower : AbstractGuZhenRenPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://images/powers/temporary_strength_down.png";
    public override string? CustomBigIconPath => "res://images/powers/temporary_strength_down.png";

    public AbstractModel OriginModel => ModelDb.Card<LiLiangGu>();

    public override LocString Title => ((CardModel)OriginModel).TitleLocString;
    public override LocString Description => new LocString("powers", "TEMPORARY_STRENGTH_DOWN.description");
    protected override string SmartDescriptionLocKey => "TEMPORARY_STRENGTH_DOWN.smartDescription";

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard((CardModel)OriginModel),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        Flash();
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<StrengthPower>(Owner, -Amount, Owner, null);
    }
}
