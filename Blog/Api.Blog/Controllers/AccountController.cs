using Api.Blog.Data;
using Api.Blog.Extensions;
using Api.Blog.Models;
using Api.Blog.Services;
using Api.Blog.ViewModels;
using Api.Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Net;
using System.Text.RegularExpressions;

namespace Api.Blog.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost("v1/accounts")]
    public async Task<IActionResult> PostAsync([FromBody] RegisterViewModel model,
        [FromServices] BlogDataContext context,
        [FromServices] EmailService emailService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var password = PasswordGenerator.Generate(25);
        var passwordHash = PasswordHasher.Hash(password);

        var user = new User(model.Name,
            model.Email,
            model.Email.Replace("@", "-").Replace(".", "-"),
            passwordHash);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            emailService.Send(user.Name, user.Email, "Bem vindo ao Blog!", $"A sua senha é {password}");
            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email,
                password
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, new ResultViewModel<string>(new List<string>() { $"Este email já está cadastrado" }));
        }
        catch
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string>() { $"Falha interna no servidor" }));
        }

        //{
        //    "data": {
        //        "user": "thiagotls58@gmail.com",
        //    "password": "(Z]Hk1Zy{0[}RTvmvzwXo)myy"
        //            },
        //  "errors": []
        //}
    }

    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model,
        [FromServices] BlogDataContext context,
        [FromServices] TokenService tokenService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = await context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
            return StatusCode((int)HttpStatusCode.Unauthorized, new ResultViewModel<string>(new List<string>() { "Usuário ou senha inválidos" }));

        bool validouSenha = user.ValidarSenha(model.Senha);
        if (!validouSenha)
            return StatusCode((int)HttpStatusCode.Unauthorized, new ResultViewModel<string>(new List<string>() { "Usuário ou senha inválidos" }));

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token));
        }
        catch
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string>() { $"Falha interna no servidor" }));
        }
    }

    [Authorize]
    [HttpPost("v1/accounts/upload-image")]
    public async Task<IActionResult> UploadImageAsync([FromBody] UploadImageViewModel model,
        [FromServices] BlogDataContext context)
    {
        string fileName = $"{Guid.NewGuid()}.jpg";
        string data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Base64Image, "");
        byte[] bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string>() { $"Falha interna no servidor" }));
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
        if (user == null)
            return NotFound(new ResultViewModel<string>(new List<string>() { "Usuário não encontrado" }));

        user.SetImage($"wwwroot/images/{fileName}");

        try
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ResultViewModel<string>(new List<string>() { $"Falha interna no servidor" }));
        }

        return Ok(new ResultViewModel<string>("Imagem alterada com sucesso!"));
    }
}
