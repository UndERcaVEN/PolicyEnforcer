using System;
using System.Collections.Generic;

namespace PolicyEnforcer.ServerCore.Database.Models;

public partial class BrowserHistory
{
    public Guid Id { get; set; }

    public string MachineName { get; set; } = null!;

    public string Url { get; set; } = null!;

    public DateTime DateVisited { get; set; }

    public string BrowserName { get; set; } = null!;
}
