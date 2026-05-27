using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenRelicPool))]
public sealed class XianGuCanHai : CustomRelicModel
{
    private const string ExtraSmithKey = "ExtraSmith";

    private int _counter = 1;

    public override RelicRarity Rarity => RelicRarity.Event;

    public override bool ShowCounter => true;

    public override int DisplayAmount => Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(ExtraSmithKey, 1m)];

    [SavedProperty]
    public int Counter
    {
        get => _counter;
        set
        {
            AssertMutable();
            _counter = Math.Max(0, value);
            DynamicVars[ExtraSmithKey].BaseValue = _counter;
            InvokeDisplayAmountChanged();
        }
    }

    public override string PackedIconPath => GuZhenRenArtPaths.GetRelicIcon("XianGuCanHai");
    protected override string PackedIconOutlinePath => GuZhenRenArtPaths.GetRelicOutline("XianGuCanHai");
    protected override string BigIconPath => GuZhenRenArtPaths.GetRelicIcon("XianGuCanHai");

    public override Task AfterObtained()
    {
        if (!IsMutable)
        {
            return Task.CompletedTask;
        }

        if (Counter <= 0)
        {
            Counter = 1;
            return Task.CompletedTask;
        }

        DynamicVars[ExtraSmithKey].BaseValue = Counter;
        return Task.CompletedTask;
    }
}

