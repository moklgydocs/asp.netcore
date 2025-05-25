namespace MiddleWare.IPFilterMiddleware
{
    public class IPFilterOption
    {
        public List<string> AllowedIps { get; set; } = new List<string>();

        public List<string> BannedIps { get; set; } = new List<string>();

        public bool AllowAllIPsIfListEmpty { get; set; } = true;
    }
}
