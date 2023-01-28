using System.ComponentModel.DataAnnotations;

namespace Api.Blog.ViewModels.Tags;

public class EditorTagViewModel
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 80 caracteres")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O slug é obrigatório")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "O slug deve conter entre 3 e 80 caracteres")]
    public string Slug { get; set; }
}
