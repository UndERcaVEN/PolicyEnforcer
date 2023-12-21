using System;
using System.Collections.Generic;

namespace PolicyEnforcer.ServerCore.Database.Models;

public partial class ClientMachine
{
    public string MachineId { get; set; } = null!;

    public string MachineName { get; set; } = null!;

    public Guid UserId { get; set; }

    public virtual ICollection<HardwareInfo> HardwareInfos { get; set; } = new List<HardwareInfo>();

    public virtual User User { get; set; } = null!;
}
