using Api.Blog.Data;
using Api.Blog.Extensions;
using Api.Blog.Models;
using Api.Blog.ViewModels;
using Api.Blog.ViewModels.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace Api.Blog.Controllers;

[ApiController]
[Authorize]
public class RoleController : ControllerBase
{
    [HttpGet("v1/roles")]
    public IActionResult Get([FromServices] IMemoryCache cache,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var roles = cache.GetOrCreate("RolesCache", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return GetRoles(context);
            });
            return Ok(new ResultViewModel<List<Role>>(roles));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string>() { "Falha interna no servidor" }));
        }
    }

    private List<Role> GetRoles(BlogDataContext context) => context.Roles.ToList();

    [HttpGet("v1/roles/{id:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
                return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

            return Ok(new ResultViewModel<Role>(role));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string>() { "Falha interna no servidor" }));
        }
    }

    [HttpPost("v1/roles")]
    public async Task<IActionResult> PostAsync([FromBody] EditorRoleViewModel model,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<List<string>>(ModelState.GetErrors()));

        try
        {
            var role = new Role(model.Name,
                model.Slug.ToLower());

            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
            return Created($"v1/roles/{role.Id}", new ResultViewModel<Role>(role));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível incluir a role" }));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
        }
    }

    [HttpPut("v1/roles/{id:int}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id,
        [FromBody] EditorRoleViewModel model,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
                return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

            role.Atualizar(model);
            context.Roles.Update(role);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Role>(role));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível alterar a role" }));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
        }
    }

    [HttpDelete("v1/roles/{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id,
        [FromServices] BlogDataContext context)
    {
        try
        {
            var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
                return NotFound(new ResultViewModel<string>(new List<string> { "Conteúdo não encontrado" }));

            context.Roles.Remove(role);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Role>(role));
        }
        catch (DbUpdateException ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Não foi possível excluir a role" }));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string> { "Falha interna no servidor" }));
        }
    }
}
