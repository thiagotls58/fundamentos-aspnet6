using Api.Blog.Data;
using Api.Blog.Extensions;
using Api.Blog.Models;
using Api.Blog.ViewModels;
using Api.Blog.ViewModels.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace Api.Blog.Controllers;

[ApiController]
[Authorize]
public class TagController : ControllerBase
{
    [HttpGet("v1/tags")]
    public IActionResult Get([FromServices] IMemoryCache cache,
       [FromServices] BlogDataContext context)
    {
		try
		{
            var tags = cache.GetOrCreate("TagsCache", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return GetTags(context);
            });
            return Ok(new ResultViewModel<List<Tag>>(tags));
		}
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string>() { "Falha interna no servidor" }));
        }
    }

    private List<Tag> GetTags(BlogDataContext context) => context.Tags.ToList();

    [HttpGet("v1/tags/{id:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) 
                return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

            return Ok(new ResultViewModel<Tag>(tag));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string>() { "Falha interna no servidor" }));
        }
    }

    [HttpPost("v1/tags")]
    public async Task<IActionResult> PostAsync([FromBody] EditorTagViewModel model,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<List<string>>(ModelState.GetErrors()));

        try
        {
            var tag = new Tag(model.Name,
                model.Slug.ToLower());

            await context.Tags.AddAsync(tag);
            await context.SaveChangesAsync();
            return Created($"v1/tags/{tag.Id}", new ResultViewModel<Tag>(tag));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível incluir a tag" }));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
        }
    }

    [HttpPut("v1/tags/{id:int}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id,
        [FromBody] EditorTagViewModel model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null)
                return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

            tag.Atualizar(model);
            context.Tags.Update(tag);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Tag>(tag));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível alterar a tag" }));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
        }
    }

    [HttpDelete("v1/tags/{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null)
                return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
            return Ok(new ResultViewModel<Tag>(tag));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível excluir a tag" }));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
        }
    }
}
