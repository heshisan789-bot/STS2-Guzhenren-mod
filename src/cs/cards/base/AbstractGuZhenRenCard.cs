using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guzhenren.Scripts;

/// <summary>
/// 蛊真人卡牌公共底座。
/// 当前先承接 rank、流派名和若干标记位，后续把塔一里的描述拼装与特殊规则逐步迁入。
/// </summary>
public abstract class AbstractGuZhenRenCard : CustomCardModel
{
    protected AbstractGuZhenRenCard(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target, true)
    {
    }

    public int BaseRank { get; private set; } = 1;
    public int Rank { get; private set; } = 1;
    public GuZhenRenDao PrimaryDao { get; private set; } = GuZhenRenDao.None;
    public string DaoName => PrimaryDao.ToDisplayName();

    public virtual bool IsBenMingGu => false;
    public virtual bool IsShaZhao => false;
    public virtual bool IsXianGu => false;
    public virtual bool IsXuYing => false;

    public override string PortraitPath => GuZhenRenArtPaths.GetCardPortrait(GetType().Name);
    public override string BetaPortraitPath => GuZhenRenArtPaths.GetCardBetaPortrait(GetType().Name);
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<GuZhenRenCardPool>();
    protected override HashSet<CardTag> CanonicalTags => [];
    public override IEnumerable<CardKeyword> CanonicalKeywords => Array.Empty<CardKeyword>();
    protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();
    protected override IEnumerable<IHoverTip> ExtraHoverTips => Array.Empty<IHoverTip>();

    protected void SetRank(int amount)
    {
        BaseRank = amount;
        Rank = amount;
        OnRankChanged();
    }

    protected void UpgradeRank(int amount)
    {
        BaseRank += amount;
        Rank = BaseRank;
        OnRankChanged();
    }

    protected void SetDao(GuZhenRenDao dao)
    {
        PrimaryDao = dao;
    }

    protected virtual void OnRankChanged()
    {
    }
}
