using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

public sealed class XueChouPower : AbstractGuZhenRenPower
{
    private sealed class Data
    {
        public ulong? ApplierNetId { get; set; }
    }

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool AllowNegative => false;
    public override int DisplayAmount => 0;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var data = GetInternalData<Data>();
        data.ApplierNetId = applier?.Player?.NetId;
        return Task.CompletedTask;
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack_())
        {
            return 1m;
        }

        var data = GetInternalData<Data>();
        if (!data.ApplierNetId.HasValue)
        {
            return 1m;
        }

        if (target == Owner && dealer?.Player?.NetId == data.ApplierNetId.Value)
        {
            return 2m;
        }

        if (dealer == Owner && target?.Player?.NetId == data.ApplierNetId.Value)
        {
            return 2m;
        }

        return 1m;
    }
}
