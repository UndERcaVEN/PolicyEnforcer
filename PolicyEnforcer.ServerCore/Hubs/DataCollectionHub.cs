using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PolicyEnforcer.ServerCore.Database.Context;
using PolicyEnforcer.ServerCore.Database.Models;
using PolicyEnforcer.ServerCore.DTO;

namespace PolicyEnforcer.ServerCore.Hubs
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
                    Temperature = piece.Temperature,
                    UserId = piece.UserID
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
                    UserId = piece.UserId,
                    BrowserName = piece.BrowserName,
                    Url = piece.Url,
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
