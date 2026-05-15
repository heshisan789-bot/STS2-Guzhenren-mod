using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.GuangDao)]
public sealed class TianPuGuangHe : AbstractShaZhaoCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TaiGuRongYaoZhiGuangPower>(3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TaiGuRongYaoZhiGuangPower>()];

    public TianPuGuangHe() : base(1, CardType.Skill, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.GuangDao);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        foreach (var enemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<TaiGuRongYaoZhiGuangPower>(enemy, DynamicVars["TaiGuRongYaoZhiGuangPower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
