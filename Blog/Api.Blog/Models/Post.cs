using Api.Blog.ViewModels.Posts;
using System.Text.Json.Serialization;

namespace Api.Blog.Models;

public class Post
{
    public Post(string title,
        string summary,
        string body,
        string slug,
        int categoryId,
        int authorId,
        int id = 0,
        DateTime createDate = default,
        DateTime lastUpdateDate = default)
    {
        Title = title;
        Summary = summary;
        Body = body;
        Slug = slug;
        CategoryId = categoryId;
        AuthorId = authorId;
        Id = id;
        CreateDate = createDate;
        LastUpdateDate = lastUpdateDate;
        Tags = new List<Tag>();
    }

    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Summary { get; private set; }
    public string Body { get; private set; }
    public string Slug { get; private set; }
    public DateTime CreateDate { get; private set; }
    public DateTime LastUpdateDate { get; private set; }
    
    public int CategoryId { get; private set; }
    public Category Category { get; private set; }

    public int AuthorId { get; private set; }
    public User Author { get; private set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Tag> Tags { get; private set; }

    public void Atualizar(EditorPostViewModel model)
    {
        Title = model.Title;
        Summary = model.Summary;
        Body = model.Body;
        Slug = model.Slug;
        CategoryId = model.CategoryId;
        AuthorId = model.AuthorId;
        LastUpdateDate = DateTime.Now;
    }
}
