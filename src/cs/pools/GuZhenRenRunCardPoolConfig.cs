namespace Guzhenren.Scripts;

/// <summary>
/// 预留给后续“本局禁用某几个道”的运行时配置。
/// 当前先落结构，不接入开局选择逻辑。
/// </summary>
public sealed class GuZhenRenRunCardPoolConfig
{
    public HashSet<GuZhenRenDao> ExcludedDaos { get; } = [];

    public bool IsDaoEnabled(GuZhenRenDao dao)
    {
        return dao == GuZhenRenDao.None || !ExcludedDaos.Contains(dao);
    }
}
