using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

[Authorize(Roles = "Viewer, Admin, Accountant")]
public class ViewerOnlyModel : PageModel
{
    //public List<Account> Accounts { get; set; }

    //private readonly IAccountService _accountService;

    //public ViewerOnlyModel(IAccountService accountService)
    //{
    //    _accountService = accountService;
    //}

    //public async Task OnGetAsync()
    //{
    //    Accounts = await _accountService.GetAllAccountsAsync();
    //}
}
