using System.ComponentModel.DataAnnotations;

namespace Api.Blog.ViewModels.Posts;

public class EditorPostViewModel
{
    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O título deve conter entre e 100 caracteres")]
    public string Title { get; set; }
    [Required(ErrorMessage = "O sumário é obrigatório")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O sumário deve conter entre e 200 caracteres")]
    public string Summary { get; set; }
    [Required(ErrorMessage = "O corpo é obrigatório")]
    [StringLength(300, MinimumLength = 3, ErrorMessage = "O corpo deve conter entre e 300 caracteres")]
    public string Body { get; set; }
    [Required(ErrorMessage = "O slug é obrigatório")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "O slug deve conter entre e 80 caracteres")]
    public string Slug { get; set; }
    [Required(ErrorMessage = "A categoria é obrigatório")]
    public int CategoryId { get; set; }
    [Required(ErrorMessage = "O autor é obrigatório")]
    public int AuthorId { get; set; }
}
