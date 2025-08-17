namespace AttechServer.Applications.UserModules.Dtos.News
{
    public class NewsGalleryDto
    {
        public int Id { get; set; }
        public string TitleVi { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string SlugVi { get; set; } = string.Empty;
        public string SlugEn { get; set; } = string.Empty;
        public NewsImageDto? FeaturedImage { get; set; }
        public List<NewsImageDto> GalleryImages { get; set; } = new();
    }

    public class NewsImageDto
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}