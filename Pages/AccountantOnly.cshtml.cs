using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Accountant, Admin")]
public class AccountantOnlyModel : PageModel
{
    public void OnGet()
    {
        // Logic for accountant goes here
    }
}
