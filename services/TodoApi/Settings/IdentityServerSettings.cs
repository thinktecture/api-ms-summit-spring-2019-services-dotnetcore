using System.Security.Policy;

namespace TodoApi.Settings
{
    public class IdentityServerSettings
    {
        public string Url { get; set; }
        public string SwaggerClientId { get; set; }
        public string SwaggerClientSecret { get; set; }
        public string PushClientId { get; set; }
        public string PushClientSecret { get; set; }
    }
}