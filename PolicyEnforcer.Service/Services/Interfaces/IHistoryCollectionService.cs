namespace PolicyEnforcer.Service.Services.Interfaces
{
    public interface IHistoryCollectionService
    {
        List<string> GetBrowsersHistory(DateTime from, Guid userID);
    }
}