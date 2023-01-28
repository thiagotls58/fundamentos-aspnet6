using Api.Blog.Models;
using Api.Blog.ViewModels.Roles;

namespace Tests.Blog.Models;

[TestClass]
public class RoleTests
{
    private Role _role;

	public RoleTests()
	{
		_role = new Role("role",
			  "slug");
	}

	[TestMethod]
	public void DeveAtualizarRole()
	{
		var model = new EditorRoleViewModel
		{
			Name = "name2",
			Slug = "slug2",
		};

		_role.Atualizar(model);

		Assert.AreEqual(model.Name, _role.Name);
		Assert.AreEqual(model.Slug, _role.Slug);
	}
}
