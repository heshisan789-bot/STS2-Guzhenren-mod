using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.BianHuaDao)]
public sealed class BianTong : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => IsUpgraded ? [CardKeyword.Retain] : [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("DaoHenPerDebuff", 1m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BianHuaDaoDaoHenPower>()];

    public BianTong() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        SetDao(GuZhenRenDao.BianHuaDao);
        SetRank(7);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var debuffs = Owner.Creature.Powers
            .Where(p => p is PowerModel { Type: PowerType.Debuff })
            .ToList();

        if (debuffs.Count == 0)
            return;

        foreach (var debuff in debuffs)
        {
            await PowerCmd.Remove(debuff);
            await ZhuanYiPower.TriggerConversion(Owner.Creature, Owner.Creature, this);
        }

        var totalDaoHen = debuffs.Count * (int)DynamicVars["DaoHenPerDebuff"].BaseValue;
        await PowerCmd.Apply<BianHuaDaoDaoHenPower>(Owner.Creature, totalDaoHen, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        UpgradeRank(1);
        AddKeyword(CardKeyword.Retain);
    }
}
