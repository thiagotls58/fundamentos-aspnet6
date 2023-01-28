using SecureIdentity.Password;

namespace Api.Blog.Models;

public class User
{
    public User(string name,
        string email,
        string slug,
        string passwordHash)
    {
        Name = name;
        Email = email;
        Slug = slug;
        PasswordHash = passwordHash;
        Posts = new List<Post>();
        Roles = new List<Role>();
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Image { get; private set; }
    public string Slug { get; private set; }
    public string Bio { get; private set; }

    public IList<Post> Posts { get; private set; }
    public IList<Role> Roles { get; private set; }

    public bool ValidarSenha(string senha) => PasswordHasher.Verify(PasswordHash, senha);
    public void SetImage(string image) => Image = image;
}
