using LibreHardwareMonitor.Hardware;
using Newtonsoft.Json;
using PolicyEnforcer.Service.HardwareMonitoring;
using PolicyEnforcer.Service.Services.Interfaces;

namespace PolicyEnforcer.Service.Services
{
    public class HardwareMonitoringService : IHardwareMonitoringService
    {
        private Computer computer;

        public HardwareMonitoringService()
        {
            UpdateComputerInfo();
        }

        /// <summary>
        /// Собирает ответ серверу
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<string> PollHardware(Guid userID)
        {
            var result = new List<string>();

            foreach (var hw in computer.Hardware)
            {
                result.AddRange(GetHardwareReadings(hw, userID));
            }

            return result;
        }

        /// <summary>
        /// Опрашивает сенсоры на АО
        /// </summary>
        /// <param name="hw">единица АО</param>
        /// <param name="userID">идентификатор пользователя</param>
        /// <returns>сериализованный результат</returns>
        private List<string> GetHardwareReadings(IHardware hw, Guid userID)
        {
            var result = new List<string>();

            var hwPiece = new HardwarePiece { InstanceName = hw.Name, UserID = userID };
            foreach (var sensor in hw.Sensors)
            {
                if (sensor.SensorType == SensorType.Temperature)
                {
                    hwPiece.Temperature = sensor.Value;
                }
                if (sensor.SensorType == SensorType.Load)
                {
                    hwPiece.Load = sensor.Value;
                }
            }

            hwPiece.TimeMeasured = DateTime.Now;

            result.Add(JsonConvert.SerializeObject(hwPiece));

            foreach (var childHw in hw.SubHardware)
            {
                result.AddRange(GetHardwareReadings(childHw, userID));
            }
            return result;
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        private void UpdateComputerInfo()
        {
            computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsStorageEnabled = true,
            };

            var visitor = new UpdateVisitor();

            computer.Open();
            computer.Accept(visitor);
        }
    }
}
