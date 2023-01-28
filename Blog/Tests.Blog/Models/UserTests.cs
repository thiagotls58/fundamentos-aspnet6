using Api.Blog.Models;
using SecureIdentity.Password;

namespace Tests.Blog.Models;

[TestClass]
public class UserTests
{
    private User _user;
    private string _password;

    public UserTests()
    {
        _password = PasswordGenerator.Generate(25);
        var passwordHash = PasswordHasher.Hash(_password);

        _user = new User("thiago",
            "email@email.com",
            "email-email-com",
            passwordHash);
    }

    [TestMethod]
    public void DeveValidarSenha()
    {
        bool validouSenha = _user.ValidarSenha(_password);
        Assert.IsTrue(validouSenha);
    }

    [TestMethod]
    public void DeveAtualizarImagem()
    {
        string imageBase64 = "imagebase64";
        _user.SetImage(imageBase64);

        Assert.AreEqual(imageBase64, _user.Image);
    }

}
