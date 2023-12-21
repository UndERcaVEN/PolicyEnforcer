using PolicyEnforcer.Interfaces;

namespace PolicyEnforcer.Service.HistoryCollection
{
    public class VisitedURL : IVisitedURL
    {
        public VisitedURL(string browserName, string url, string date)
        {
            BrowserName = browserName;
            MachineID = Environment.MachineName;
            Url = url;

            // Chromium-браузеры сохраняют дату в формате Unix, с отсчетом от 1601 года
            DateTime dateVisited = new(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateVisited = dateVisited.AddSeconds(Double.Parse(date) / 1000000).ToLocalTime();
        }

        public string Url { get; set; }
        public string MachineID { get; set; }
        public string BrowserName { get; set; }
        public DateTime DateVisited { get; set; }

        public override string ToString()
        {
            return $"Browser: {BrowserName} URL: {Url} Machine: {MachineID} Date: {DateVisited}";
        }
    }
}
