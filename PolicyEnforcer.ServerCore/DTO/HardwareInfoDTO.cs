namespace PolicyEnforcer.ServerCore.DTO
{
    public class HardwareInfoDTO
    {
        public string InstanceName { get; set; }
        public double? Temperature { get; set; }
        public double? Load { get; set; }
        public DateTime DateMeasured { get; set; }
    }
}
