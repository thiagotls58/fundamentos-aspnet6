using Api.Blog.Data;
using Api.Blog.Extensions;
using Api.Blog.Models;
using Api.Blog.ViewModels;
using Api.Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Blog.Controllers;

[ApiController]
[Authorize]
public class PostController : ControllerBase
{
	[HttpGet("v1/posts")]
	public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context,
		[FromQuery] int page = 0,
		[FromQuery] int pageSize = 25)
	{
		try
		{
			int count = await context.Posts.AsNoTracking().CountAsync();
			var posts = await context.Posts
				.AsNoTracking()
				.Include(p => p.Author)
				.Include(p => p.Category)
				.Select(p => new ListPostsViewModel
				{
					Id = p.Id,
					Title = p.Title,
					Slug = p.Slug,
					LastUpdateDate = p.LastUpdateDate,
					Category = p.Category.Name,
					Author = $"{p.Author.Name} ({p.Author.Email})",
				})
				.Skip(page * pageSize)
				.Take(pageSize)
				.OrderByDescending(p => p.LastUpdateDate)
				.ToListAsync();

			return Ok(new ResultViewModel<dynamic>(new
			{
				total = count,
				page,
				pageSize,
				posts
			}));

		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<List<string>>(new List<string> { "Falha interna no servidor" }));
		}
	}

	[HttpGet("v1/posts/{id:int}")]
	public async Task<IActionResult> DetailsAsync([FromRoute] int id,
		[FromServices] BlogDataContext context)
	{
		try
		{
			var post = await context.Posts
				.AsNoTracking()
				.Include(p => p.Author)
					.ThenInclude(a => a.Roles)
				.Include(p => p.Category)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (post == null)
				return NotFound(new ResultViewModel<List<string>>(new List<string> { "Conteúdo não encontrado" }));

			return Ok(new ResultViewModel<Post>(post));
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<List<string>>(new List<string> { "Falha interna no servidor" }));
		}
	}

	[HttpGet("v1/posts/category/{category}")]
	public async Task<IActionResult> GetByCategoryAsync([FromRoute] string category,
		[FromServices] BlogDataContext context,
		[FromQuery] int page = 0,
		[FromQuery] int pageSize = 25)
	{
		try
		{
			int count = await context.Posts.AsNoTracking().CountAsync();

			var posts = await context.Posts
				.AsNoTracking()
				.Include(p => p.Author)
				.Include(p => p.Category)
				.Where(p => p.Category.Name.Contains(category))
				   .Select(p => new ListPostsViewModel
				   {
					   Id = p.Id,
					   Title = p.Title,
					   Slug = p.Slug,
					   LastUpdateDate = p.LastUpdateDate,
					   Category = p.Category.Name,
					   Author = $"{p.Author.Name} ({p.Author.Email})",
				   })
				   .ToListAsync();

			return Ok(new ResultViewModel<dynamic>(new
			{
				total = count,
				page,
				pageSize,
				posts
			}));
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<List<string>>(new List<string> { "Falha interna no servidor" }));
		}
	}

	[HttpPost("v1/posts")]
	public async Task<IActionResult> PostAsync([FromBody] EditorPostViewModel model,
		[FromServices] BlogDataContext context)
	{
		if (!ModelState.IsValid)
			return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

		try
		{
			var post = new Post(model.Title,
				 model.Summary,
				 model.Body,
				 model.Slug,
				 model.CategoryId,
				 model.AuthorId);

			await context.Posts.AddAsync(post);
			await context.SaveChangesAsync();

			return Created($"v1/posts/{post.Id}", new ResultViewModel<Post>(post));
		}
		catch (DbUpdateException ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível incluir o post" }));
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
		}
	}

	[HttpPut("v1/posts/{id:int}")]
	public async Task<IActionResult> PutAsync([FromRoute] int id,
		[FromBody] EditorPostViewModel model,
		[FromServices] BlogDataContext context)
	{
		try
		{
			var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == id);
			if (post == null)
				return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

			post.Atualizar(model);

			context.Posts.Update(post);
			await context.SaveChangesAsync();

			return Ok(new ResultViewModel<Post>(post));
		}
		catch (DbUpdateException ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível alterar o post" }));
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
		}
	}

	[HttpDelete("v1/posts/{id:int}")]
	public async Task<IActionResult> DeleteAsync([FromRoute] int id,
		[FromServices] BlogDataContext context)
	{
		try
		{
			var post = await context.Posts.FirstOrDefaultAsync(x => x.Id == id);
			if (post == null)
				return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

			context.Posts.Remove(post);
			await context.SaveChangesAsync();
			return Ok(new ResultViewModel<Post>(post));
		}
		catch (DbUpdateException ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível alterar o post" }));
		}
		catch (Exception ex)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
		}
	}
}
