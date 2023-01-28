using Api.Blog.Models;
using Api.Blog.ViewModels.Posts;

namespace Tests.Blog.Models;

[TestClass]
public class PostTests
{
    private Post _post;
	public PostTests()
	{
		_post = new Post("Título",
			"summary",
			"body",
			"slug",
			1,
			1);
	}

	[TestMethod]
	public void DeveAtualizarPost()
	{
		var model = new EditorPostViewModel
		{
			Title = "Title2",
			Summary = "Summary2",
			Body = "body2",
			Slug = "slug2",
			CategoryId = 2,
			AuthorId = 2,
		};

		var dataAtual = DateTime.Now;
		_post.Atualizar(model);

		Assert.AreEqual(model.Title, _post.Title);
		Assert.AreEqual(model.Summary, _post.Summary);
		Assert.AreEqual(model.Body, _post.Body);
		Assert.AreEqual(model.Slug, _post.Slug);
		Assert.AreEqual(model.CategoryId, _post.CategoryId);
		Assert.AreEqual(model.AuthorId, _post.AuthorId);

		var result = DateTime.Compare(dataAtual, _post.LastUpdateDate);
		Assert.AreEqual(-1, result);
	}
}
