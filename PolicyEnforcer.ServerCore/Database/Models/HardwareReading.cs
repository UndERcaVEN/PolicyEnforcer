using System;
using System.Collections.Generic;

namespace PolicyEnforcer.ServerCore.Database.Models;

public partial class HardwareReading
{
    public string InstanceName { get; set; } = null!;

    public double? Temperature { get; set; }

    public double? Load { get; set; }

    public string MachineName { get; set; } = null!;

    public DateTime DateMeasured { get; set; }

    public Guid Id { get; set; }
}
