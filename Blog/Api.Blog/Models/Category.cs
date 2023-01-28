using Api.Blog.ViewModels.Categories;
using System.Text.Json.Serialization;

namespace Api.Blog.Models;

public class Category
{
    public Category(string name, 
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
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Post> Posts { get; set; }

    public void Atualizar(EditorCategoryViewModel model)
    {
        Name = model.Name;
        Slug = model.Slug;
    }
}
