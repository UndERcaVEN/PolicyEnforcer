namespace PolicyEnforcer.ServerCore
{
    public class Dictionaries
    {
        public static Dictionary<int, string> Roles => new()
        {
            { 0, "user" },
            { 1, "admin" }
        };
    }
}
