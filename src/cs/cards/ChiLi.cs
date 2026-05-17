using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.ShiDao)]
public sealed class ChiLi : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];

    public ChiLi() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        SetDao(GuZhenRenDao.ShiDao);
        SetRank(6);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var results = await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this);
        var killed = results.FirstOrDefault()?.WasTargetKilled ?? false;
        if (!killed || cardPlay.Target.HasPower<MinionPower>())
        {
            return;
        }

        var relic = Owner.Relics.OfType<LiDaoDaoHen>().FirstOrDefault();
        if (relic == null)
        {
            relic = (LiDaoDaoHen)await RelicCmd.Obtain<LiDaoDaoHen>(Owner);
        }

        relic.Counter += 1;
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        UpgradeRank(1);
    }
}
