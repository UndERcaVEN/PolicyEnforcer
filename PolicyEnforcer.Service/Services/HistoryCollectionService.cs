using Newtonsoft.Json;
using PolicyEnforcer.Service.Extensions;
using PolicyEnforcer.Service.HistoryCollection;
using PolicyEnforcer.Service.Services.Interfaces;
using System.Data.SQLite;

namespace PolicyEnforcer.Service.Services
{
    public class HistoryCollectionService : IHistoryCollectionService
    {
        private static string AppDataLocal => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static string AppDataRoaming => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public List<string> GetBrowsersHistory(DateTime from)
        {
            var conStrings = GetHistoryFiles();

            var result = new List<string>();
            foreach (var con in conStrings)
            {
                string tempFilePath = Path.Combine(Environment.CurrentDirectory, DateTime.Now.Ticks.ToString());
                File.Copy(con.DBPath, tempFilePath);

                try
                {
                    using (var connection = new SQLiteConnection($"Data Source={tempFilePath}", true))
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        connection.Open();

                        const long secondsBetween19701601 = 11644473600;
                        var timestamp = ((DateTimeOffset)from).ToUnixTimeSeconds() + secondsBetween19701601;
                        command.CommandText = $"select * from urls where last_visit_time >= {timestamp * 1000000}";

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Структура БД истории посещений в chromium-браузерах унифицирована
                                var url = new VisitedURL(con.BrowserName, reader["url"].ToString(), reader["last_visit_time"].ToString());
                                result.Add(JsonConvert.SerializeObject(url));
                            }
                        }
                        connection.Close();
                    }
                }
                finally
                {
                    SQLiteConnection.ClearAllPools();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(tempFilePath);
                }
            }

            return result;
        }

        private List<BrowserModel> GetHistoryFiles()
        {
            var result = new List<BrowserModel>();

            result.AddBrowser(AppDataLocal, "Google\\Chrome\\User Data\\Default\\History", "Google Chrome");
            result.AddBrowser(AppDataRoaming, "Opera Software\\Opera GX Stable\\History", "Opera GX");
            result.AddBrowser(AppDataRoaming, "Opera Software\\Opera Stable\\History", "Opera");
            result.AddBrowser(AppDataLocal, "Microsoft\\Edge\\User Data\\Default\\History", "Micrososft Edge");

            return result;
        }
    }
}
