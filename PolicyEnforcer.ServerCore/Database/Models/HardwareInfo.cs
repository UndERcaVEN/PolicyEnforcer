namespace PolicyEnforcer.ServerCore.Database.Models;

public partial class HardwareInfo
{
    public string InstanceName { get; set; } = null!;

    public double? Temperature { get; set; }

    public double? Load { get; set; }

    public DateTime DateMeasured { get; set; }

    public Guid UserId { get; set; }

    public Guid MeasurementId { get; set; }

    public virtual User User { get; set; } = null!;
}
