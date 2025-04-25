using AttechServer.Applications.UserModules.Dtos.Post;

namespace AttechServer.Applications.UserModules.Dtos.PostCategory
{
    public class DetailPostCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PostDto> Posts { get; set; } = new();
    }
}
