using PolicyEnforcer.Interfaces;

namespace PolicyEnforcer.ServerCore.DTO
{
    public class HardwarePiece : IHardwarePiece
    {
        public string InstanceName { get; set; }
        public Guid UserID { get; set; }
        public float? Temperature { get; set; }
        public float? Load { get; set; }
        public DateTime TimeMeasured { get; set; }
    }
}
