namespace AttechServer.Applications.UserModules.Dtos.Post
{
    public class CreatePostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int PostCategoryId { get; set; }
        public DateTime TimePosted { get; set; }
    }
}
