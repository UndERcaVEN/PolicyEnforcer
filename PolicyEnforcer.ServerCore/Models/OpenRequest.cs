namespace PolicyEnforcer.ServerCore.Models
{
    public class OpenRequest
    {
        public OpenRequest(IEnumerable<string> clients)
        {
            WaitFrom = clients.ToList();
        }

        public bool RemoveClient(string clientID)
        {
            WaitFrom.Remove(clientID);
            return WaitFrom.Count() > 0;
        }

        private List<string> WaitFrom;
        public string ID { get; set; }
        public string CallerID { get; set; }
    }
}
