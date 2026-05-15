using System.Reflection;

namespace Guzhenren.Scripts;

/// <summary>
/// 蛊真人模组内部卡牌目录。
/// - 自动收集所有非抽象蛊真人卡牌类型。
/// - 自动按道归档为多个逻辑子池。
/// - 后续若要实现“本局禁用某几个道”，只需基于这里的结果做筛选。
/// </summary>
public static class GuZhenRenCardCatalog
{
    private static readonly Lazy<IReadOnlyList<Type>> _allCardTypes = new(DiscoverAllCardTypes);
    private static readonly Lazy<IReadOnlyDictionary<GuZhenRenDao, GuZhenRenLogicalCardPool>> _daoPools = new(BuildDaoPools);

    public static IReadOnlyList<Type> AllCardTypes => _allCardTypes.Value;
    public static IReadOnlyDictionary<GuZhenRenDao, GuZhenRenLogicalCardPool> DaoPools => _daoPools.Value;

    public static IReadOnlyList<Type> GetCardTypes(GuZhenRenDao dao)
    {
        return DaoPools.TryGetValue(dao, out var pool) ? pool.CardTypes : Array.Empty<Type>();
    }

    public static IReadOnlyList<Type> GetEnabledCardTypes(GuZhenRenRunCardPoolConfig config)
    {
        return AllCardTypes
            .Where(type => config.IsDaoEnabled(GetDao(type)))
            .ToArray();
    }

    public static GuZhenRenDao GetDao(Type cardType)
    {
        return cardType.GetCustomAttribute<GuZhenRenDaoPoolAttribute>()?.Dao ?? GuZhenRenDao.None;
    }

    private static IReadOnlyList<Type> DiscoverAllCardTypes()
    {
        return typeof(Entry).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && typeof(AbstractGuZhenRenCard).IsAssignableFrom(type))
            .OrderBy(type => type.Name)
            .ToArray();
    }

    private static IReadOnlyDictionary<GuZhenRenDao, GuZhenRenLogicalCardPool> BuildDaoPools()
    {
        return AllCardTypes
            .GroupBy(GetDao)
            .OrderBy(group => group.Key)
            .ToDictionary(
                group => group.Key,
                group => new GuZhenRenLogicalCardPool(group.Key, group.ToArray())
            );
    }
}
