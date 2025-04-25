namespace AttechServer.Applications.UserModules.Dtos.Post
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TimePosted { get; set; }
        public int Status { get; set; }
        public int PostCategoryId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
