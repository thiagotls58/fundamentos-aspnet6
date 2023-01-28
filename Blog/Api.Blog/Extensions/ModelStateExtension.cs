using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api.Blog.Extensions;

public static class ModelStateExtension
{
    public static List<string> GetErrors(this ModelStateDictionary modelState)
    {
        var result = new List<string>();
        foreach (var item in modelState.Values)
        {
            var errors = item.Errors.Select(error => error.ErrorMessage);
            result.AddRange(errors);
        }
        return result;
    }
}
