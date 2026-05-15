using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.YanDao)]
public sealed class LiaoYuanHuo : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Times", 2m),
        new PowerVar<FenShaoPower>(3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FenShaoPower>(),
        HoverTipFactory.FromCard<Burn>()
    ];

    public LiaoYuanHuo() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        SetDao(GuZhenRenDao.YanDao);
        SetRank(5);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState, nameof(CombatState));

        for (var i = 0; i < DynamicVars["Times"].IntValue; i++)
        {
            foreach (var enemy in CombatState.HittableEnemies)
            {
                await PowerCmd.Apply<FenShaoPower>(enemy, DynamicVars["FenShaoPower"].BaseValue, Owner.Creature, this);
            }
        }

        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Burn>(Owner), PileType.Hand, addedByPlayer: true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Times"].UpgradeValueBy(1m);
        DynamicVars["FenShaoPower"].UpgradeValueBy(-1m);
        UpgradeRank(1);
    }
}
