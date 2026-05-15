namespace Guzhenren.Scripts;

public sealed class GuZhenRenLogicalCardPool
{
    public GuZhenRenLogicalCardPool(GuZhenRenDao dao, IReadOnlyList<Type> cardTypes)
    {
        Dao = dao;
        CardTypes = cardTypes;
    }

    public GuZhenRenDao Dao { get; }
    public string Id => Dao.ToPoolKey();
    public string Title => Dao.ToDisplayName();
    public IReadOnlyList<Type> CardTypes { get; }
}
