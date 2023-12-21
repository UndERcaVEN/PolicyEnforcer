using System;
using System.Collections.Generic;

namespace PolicyEnforcer.ServerCore.Database.Models;

public partial class BrowserHistory
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Url { get; set; } = null!;

    public DateTime DateVisited { get; set; }

    public string BrowserName { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
