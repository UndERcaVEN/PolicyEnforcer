using System;
using System.Collections.Generic;

namespace PolicyEnforcer.ServerCore.Database.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string Login { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public int AccessLevel { get; set; }

    public virtual ICollection<ClientMachine> ClientMachines { get; set; } = new List<ClientMachine>();
}
