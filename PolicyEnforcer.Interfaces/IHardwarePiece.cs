using System;

namespace PolicyEnforcer.Interfaces
{
    public interface IHardwarePiece
    {
        string InstanceName { get; set; }
        Guid UserID { get; set; }
        float? Temperature { get; set; }
        float? Load { get; set; }
        DateTime TimeMeasured { get; set; }
    }
}
