namespace AttechServer.Applications.UserModules.Dtos.Menu
{
    public class CreateMenuDto
    {
        public string Key { get; set; } = string.Empty;
        public string LabelVi { get; set; } = string.Empty;
        public string LabelEn { get; set; } = string.Empty;
        public string? PathVi { get; set; }
        public string? PathEn { get; set; }
        public int? ParentId { get; set; }
    }
} 