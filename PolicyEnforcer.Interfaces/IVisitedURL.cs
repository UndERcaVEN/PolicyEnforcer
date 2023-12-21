using System;

namespace PolicyEnforcer.Interfaces
{
    public interface IVisitedURL
    {
        string Url { get; set; }
        string MachineID { get; set; }
        string BrowserName { get; set; }
        DateTime DateVisited { get; set; }
    }
}
