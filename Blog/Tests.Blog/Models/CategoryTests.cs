using Api.Blog.Models;
using Api.Blog.ViewModels.Categories;

namespace Tests.Blog.Models;

[TestClass]
public class CategoryTests
{
    private Category _category;

    public CategoryTests()
    {
        _category = new Category("categoria 1", "categoria-1");
    }

    [TestMethod]
    public void DeveAtualizarCategoria()
    {
        var model = new EditorCategoryViewModel
        {
            Name = "categoria 2",
            Slug = "categoria-2"
        };

        _category.Atualizar(model);

        Assert.AreEqual(model.Name, _category.Name);
        Assert.AreEqual(model.Slug, _category.Slug);
    }
}
