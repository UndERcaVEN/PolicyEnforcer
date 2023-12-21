using System;
using System.Collections.Generic;

namespace PolicyEnforcer.ServerCore.Database.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string Login { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public int AccessLevel { get; set; }

    public virtual ICollection<BrowserHistory> BrowserHistories { get; set; } = new List<BrowserHistory>();

    public virtual ICollection<HardwareInfo> HardwareInfos { get; set; } = new List<HardwareInfo>();
}
