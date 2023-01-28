using Api.Blog.Models;
using Api.Blog.ViewModels.Tags;

namespace Tests.Blog.Models;

[TestClass]
public class TagTests
{
    private Tag _tag;
    public TagTests()
    {
        _tag = new Tag("tag",
            "slug");
    }

    [TestMethod]
    public void DeveAtualizarTag()
    {
        var model = new EditorTagViewModel
        {
            Name = "name2",
            Slug = "slug2",
        };

        _tag.Atualizar(model);

        Assert.AreEqual(model.Name, _tag.Name);
        Assert.AreEqual(_tag.Slug, _tag.Slug);
    }
}
