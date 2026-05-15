using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LuDao)]
public sealed class WuJinXuanGuangQi : AbstractShaZhaoCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<SlowPower>()
    ];

    public WuJinXuanGuangQi() : base(2, CardType.Skill, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.LuDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<WeakPower>(enemy, 99m, Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(enemy, 99m, Owner.Creature, this);
            await PowerCmd.Apply<SlowPower>(enemy, 0m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
