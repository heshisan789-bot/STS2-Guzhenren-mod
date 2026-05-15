using System.Reflection;

namespace Guzhenren.Scripts;

/// <summary>
/// 标记一张卡所属的“道”子池。
/// 当前只用于模组内部逻辑分组，不改变塔二原生的角色总卡池结构。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GuZhenRenDaoPoolAttribute : Attribute
{
    public GuZhenRenDaoPoolAttribute(GuZhenRenDao dao)
    {
        Dao = dao;
    }

    public GuZhenRenDao Dao { get; }
}
