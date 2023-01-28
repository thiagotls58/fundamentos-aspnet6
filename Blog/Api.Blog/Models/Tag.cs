using Api.Blog.ViewModels.Tags;

namespace Api.Blog.Models; 
public class Tag 
{
    public Tag(string name,
        string slug,
        int id = 0)
    {
        Name = name;
        Slug = slug;
        Id = id;
        Posts = new List<Post>();
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }

    public IList<Post> Posts { get; private set; }

    public void Atualizar(EditorTagViewModel model)
    {
        Name = model.Name;
        Slug = model.Slug;
    }
}
