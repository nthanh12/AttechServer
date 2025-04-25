namespace AttechServer.Applications.UserModules.Dtos.Post
{
    public class DetailPostDto
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime TimePosted { get; set; }
        public int Status { get; set; }
        public int PostCategoryId { get; set; }
        public string PostCategoryName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
