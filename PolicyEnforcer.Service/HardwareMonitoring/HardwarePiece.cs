using PolicyEnforcer.Interfaces;

namespace PolicyEnforcer.Service.HardwareMonitoring
{
    internal class HardwarePiece : IHardwarePiece
    {
        public string InstanceName { get; set; }
        public string MachineID { get; set; }
        public float? Temperature { get; set; }
        public float? Load { get; set; }
        public DateTime TimeMeasured { get; set; }
    }
}
