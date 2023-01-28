using Api.Blog.Data;
using Api.Blog.Extensions;
using Api.Blog.Models;
using Api.Blog.ViewModels;
using Api.Blog.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace Api.Blog.Controllers;

[ApiController]
[Authorize]
public class CategoryController : ControllerBase
{
	[HttpGet("v1/categories")]
	public IActionResult Get([FromServices] IMemoryCache cache,
		[FromServices] BlogDataContext context)
	{
		try
		{
			var categories = cache.GetOrCreate("CategoriesCache", entry =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
				return GetCategories(context);
			});
			return Ok(new ResultViewModel<List<Category>>(categories));
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string>() { "Falha interna no servidor" }));
		}
	}

	private List<Category> GetCategories(BlogDataContext context) => context.Categories.ToList();

	[HttpGet("v1/categories/{id:int}")]
	public async Task<IActionResult> GetByIdAsync([FromRoute] int id,
		[FromServices] BlogDataContext context)
	{
		try
		{
			var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

			if (category == null)
				return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

			return Ok(new ResultViewModel<Category>(category));
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
		}
	}

	[HttpPost("v1/categories")]
	public async Task<IActionResult> PostAsync([FromBody] EditorCategoryViewModel model,
		[FromServices] BlogDataContext context)
	{
		if (!ModelState.IsValid)
			return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

		try
		{
			var category = new Category(model.Name,
				model.Slug.ToLower());

			await context.Categories.AddAsync(category);
			await context.SaveChangesAsync();

			return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
		}
		catch (DbUpdateException ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível incluir a categoria" }));
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
		}
	}

	[HttpPut("v1/categories/{id:int}")]
	public async Task<IActionResult> PutAsync([FromRoute] int id,
		[FromBody] EditorCategoryViewModel model,
		[FromServices] BlogDataContext context)
	{
		try
		{
			var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

			if (category == null)
				return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

			category.Atualizar(model);

			context.Categories.Update(category);
			await context.SaveChangesAsync();

			return Ok(new ResultViewModel<Category>(category));
		}
		catch (DbUpdateException ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível alterar a categoria" }));
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
		}
	}

	[HttpDelete("v1/categories/{id:int}")]
	public async Task<IActionResult> DeleteAsync([FromRoute] int id,
		[FromServices] BlogDataContext context)
	{
		try
		{
			var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

			if (category == null)
				return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

			context.Categories.Remove(category);
			await context.SaveChangesAsync();

			return Ok(new ResultViewModel<Category>(category));
		}
		catch (DbUpdateException ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível excluir a categoria" }));
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
		}
	}
}
