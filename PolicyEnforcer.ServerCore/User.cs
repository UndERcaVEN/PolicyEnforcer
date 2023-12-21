using System;
using System.Collections.Generic;

namespace PolicyEnforcer.ServerCore;

public partial class User
{
    public Guid UserId { get; set; }

    public string Login { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public int AccessLevel { get; set; }
}
