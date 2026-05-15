using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ZhiDao)]
public sealed class ZhanNianGu : AbstractGuZhenRenCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new PowerVar<NianPower>(4m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NianPower>()];

    public ZhanNianGu() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.ZhiDao);
        SetRank(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (WasPreviousCardAttack())
        {
            await PowerCmd.Apply<NianPower>(Owner.Creature, DynamicVars["NianPower"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["NianPower"].UpgradeValueBy(1m);
        UpgradeRank(1);
    }

    private bool WasPreviousCardAttack()
    {
        if (CombatState == null)
        {
            return false;
        }

        CardModel? previousCard = null;
        foreach (var entry in CombatManager.Instance.History.CardPlaysFinished)
        {
            if (entry.HappenedThisTurn(CombatState) && entry.CardPlay.Card.Owner == Owner)
            {
                previousCard = entry.CardPlay.Card;
            }
        }

        return previousCard?.Type == CardType.Attack;
    }
}
