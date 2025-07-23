namespace AttechServer.Applications.UserModules.Dtos.Menu
{
    public class UpdateMenuDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string LabelVi { get; set; } = string.Empty;
        public string LabelEn { get; set; } = string.Empty;
        public string? PathVi { get; set; }
        public string? PathEn { get; set; }
        public int? ParentId { get; set; }
    }
} 