using PolicyEnforcer.Service.HistoryCollection;

namespace PolicyEnforcer.Service.Extensions
{
    public static class ListExtensions
    {
        public static void AddBrowser(this List<BrowserModel> arg, string appData, string historyPath, string browserName)
        {
            string path = Path.Combine(appData, historyPath);
            if (File.Exists(path))
            {
                arg.Add(new BrowserModel { BrowserName = browserName, DBPath = path });
            }
        }
    }
}
