namespace AttechServer.Applications.UserModules.Dtos
{
    public class TranslateRequest
    {
        public string Text { get; set; } = string.Empty;
        public string Source { get; set; } = "vi";
        public string Target { get; set; } = "en";
    }
}
