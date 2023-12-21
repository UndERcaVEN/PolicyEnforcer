using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PolicyEnforcer.Interfaces;
using PolicyEnforcer.ServerCore.Database.Context;
using PolicyEnforcer.ServerCore.Database.Models;
using PolicyEnforcer.ServerCore.Models;

namespace PolicyEnforcer.ServerCore
{
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIDs { get; set; } = new();
    }

    public class DataCollectionHub : Hub
    {
        private PolicyEnforcerContext _context;
        public DataCollectionHub(PolicyEnforcerContext dbContext)
        {
            _context = dbContext;
        }
        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIDs.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            UserHandler.ConnectedIDs.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        private class HardwarePiece : IHardwarePiece
        {
            public string InstanceName { get; set; }
            public string MachineID { get; set; }
            public float? Temperature { get; set; }
            public float? Load { get; set; }
            public DateTime TimeMeasured { get; set; }
        }

        public async Task ReturnHardwareReadings(List<string> readings)
        {
            foreach (var pieceRaw in readings)
            {
                var piece = JsonConvert.DeserializeObject<HardwarePiece>(pieceRaw);
                _context.HardwareInfos.Add(new()
                {
                    DateMeasured = piece.TimeMeasured,
                    MeasurementId = Guid.NewGuid(),
                    InstanceName = piece.InstanceName,
                    Load = piece.Load,
                    MachineId = piece.MachineID,
                    Temperature = piece.Temperature
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task ReturnBrowserHistory(List<string> readings)
        {
            foreach (var pieceRaw in readings)
            {
                var piece = JsonConvert.DeserializeObject<BrowserHistory>(pieceRaw);
                _context.BrowserHistories.Add(new()
                {
                    DateVisited = piece.DateVisited,
                    Id = Guid.NewGuid(),
                    BrowserName = piece.BrowserName,
                    MachineName = piece.MachineName,
                    Url = piece.Url,
                });
            }

            await _context.SaveChangesAsync();
        }

        public async void RequestBrowserHistory()
        {
            await Clients.All.SendAsync("GetBrowserHistory", 10);
        }

        public async void RequestHardwareReadings()
        {
            await Clients.All.SendAsync("GetHardwareInfo");
        }
    }
}
