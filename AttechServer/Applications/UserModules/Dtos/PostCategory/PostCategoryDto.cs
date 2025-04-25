using AttechServer.Applications.UserModules.Dtos.Post;

namespace AttechServer.Applications.UserModules.Dtos.PostCategory
{
    public class PostCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}
