using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guzhenren.Scripts;

[Pool(typeof(GuZhenRenCardPool))]
[GuZhenRenDaoPool(GuZhenRenDao.LiDao)]
public sealed class QuanLiYiFuGu : AbstractGuZhenRenCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => IsUpgraded ? [CardKeyword.Retain] : [];

    public QuanLiYiFuGu() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        SetDao(GuZhenRenDao.LiDao);
        SetRank(5);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.GetPower<QuanLiYiFuPower>() == null)
        {
            await PowerCmd.Apply<QuanLiYiFuPower>(Owner.Creature, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        if (!IsCanonical)
        {
            AddKeyword(CardKeyword.Retain);
        }

        UpgradeRank(1);
    }
}
