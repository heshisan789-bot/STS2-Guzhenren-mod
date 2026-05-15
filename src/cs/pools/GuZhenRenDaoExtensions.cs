namespace Guzhenren.Scripts;

public static class GuZhenRenDaoExtensions
{
    public static string ToDisplayName(this GuZhenRenDao dao) => dao switch
    {
        GuZhenRenDao.GuangDao => "光道",
        GuZhenRenDao.YanDao => "炎道",
        GuZhenRenDao.LiDao => "力道",
        GuZhenRenDao.JinDao => "金道",
        GuZhenRenDao.TouDao => "偷道",
        GuZhenRenDao.MuDao => "木道",
        GuZhenRenDao.ShiDao => "食道",
        GuZhenRenDao.ShaDao => "杀道",
        GuZhenRenDao.GuDao => "蛊道",
        GuZhenRenDao.LuDao => "律道",
        GuZhenRenDao.ZhiDao => "智道",
        GuZhenRenDao.BianHuaDao => "变化道",
        GuZhenRenDao.YinYangDao => "阴阳道",
        GuZhenRenDao.JianDao => "剑道",
        GuZhenRenDao.XueDao => "血道",
        GuZhenRenDao.YunDao => "运道",
        GuZhenRenDao.FengDao => "风道",
        GuZhenRenDao.ZhouDao => "宙道",
        _ => "无道",
    };

    public static string ToPoolKey(this GuZhenRenDao dao) => dao switch
    {
        GuZhenRenDao.GuangDao => "guang_dao",
        GuZhenRenDao.YanDao => "yan_dao",
        GuZhenRenDao.LiDao => "li_dao",
        GuZhenRenDao.JinDao => "jin_dao",
        GuZhenRenDao.TouDao => "tou_dao",
        GuZhenRenDao.MuDao => "mu_dao",
        GuZhenRenDao.ShiDao => "shi_dao",
        GuZhenRenDao.ShaDao => "sha_dao",
        GuZhenRenDao.GuDao => "gu_dao",
        GuZhenRenDao.LuDao => "lu_dao",
        GuZhenRenDao.ZhiDao => "zhi_dao",
        GuZhenRenDao.BianHuaDao => "bian_hua_dao",
        GuZhenRenDao.YinYangDao => "yin_yang_dao",
        GuZhenRenDao.JianDao => "jian_dao",
        GuZhenRenDao.XueDao => "xue_dao",
        GuZhenRenDao.YunDao => "yun_dao",
        GuZhenRenDao.FengDao => "feng_dao",
        GuZhenRenDao.ZhouDao => "zhou_dao",
        _ => "misc",
    };
}
