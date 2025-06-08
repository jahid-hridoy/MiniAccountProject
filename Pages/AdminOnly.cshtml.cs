using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Admin")]
public class AdminOnlyModel : PageModel
{
    public void OnGet()
    {
    }
}
