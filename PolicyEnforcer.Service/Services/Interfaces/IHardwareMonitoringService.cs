namespace PolicyEnforcer.Service.Services.Interfaces
{
    public interface IHardwareMonitoringService
    {
        List<string> PollHardware(Guid userID);
    }
}