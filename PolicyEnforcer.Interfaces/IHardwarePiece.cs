using System;

namespace PolicyEnforcer.Interfaces
{
    public interface IHardwarePiece
    {
        string InstanceName { get; set; }
        string MachineID { get; set; }
        float? Temperature { get; set; }
        float? Load { get; set; }
        DateTime TimeMeasured { get; set; }
    }
}
