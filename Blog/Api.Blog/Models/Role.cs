using Api.Blog.ViewModels.Roles;
using System.Text.Json.Serialization;

namespace Api.Blog.Models;

public class Role
{
    public Role(string name, 
        string slug, 
        int id = 0)
    {
        Name = name;
        Slug = slug;
        Id = id;
        Users = new List<User>();
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<User> Users { get; private set; }

    public void Atualizar(EditorRoleViewModel model)
    {
        Name = model.Name;
        Slug = model.Slug;
    }
}
