namespace Guzhenren.Scripts;

/// <summary>
/// 战斗生命周期事件集线器。
/// 先保留塔一的简单结构，后续逐步把复杂系统迁入此处。
/// </summary>
public static class BattleStateManager
{
    private static readonly List<Action> BattleStartActions = [];
    private static readonly List<Action> PostBattleActions = [];

    public static void OnBattleStart(Action action) => BattleStartActions.Add(action);
    public static void OnPostBattle(Action action) => PostBattleActions.Add(action);

    public static void PublishBattleStart()
    {
        foreach (var action in BattleStartActions)
        {
            action();
        }
    }

    public static void PublishPostBattle()
    {
        foreach (var action in PostBattleActions)
        {
            action();
        }
    }
}
