namespace Guzhenren.Scripts;

/// <summary>
/// 蛊真人卡牌的逻辑流派池。
/// 对外仍挂在同一个角色卡池上，对内按“道”拆分为多个子池，便于后续做运行时筛选。
/// </summary>
public enum GuZhenRenDao
{
    None = 0,
    GuangDao,
    YanDao,
    LiDao,
    JinDao,
    TouDao,
    MuDao,
    ShiDao,
    ShaDao,
    GuDao,
    LuDao,
    ZhiDao,
    BianHuaDao,
    YinYangDao,
    JianDao,
    XueDao,
    YunDao,
    FengDao,
    ZhouDao,
}
