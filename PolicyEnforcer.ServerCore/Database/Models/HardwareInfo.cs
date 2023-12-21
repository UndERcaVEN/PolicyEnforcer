using System;
using System.Collections.Generic;

namespace PolicyEnforcer.ServerCore.Database.Models;

public partial class HardwareInfo
{
    public string InstanceName { get; set; } = null!;

    public double? Temperature { get; set; }

    public double? Load { get; set; }

    public DateTime DateMeasured { get; set; }

    public string MachineId { get; set; } = null!;

    public Guid MeasurementId { get; set; }

    public virtual ClientMachine Machine { get; set; } = null!;
}
