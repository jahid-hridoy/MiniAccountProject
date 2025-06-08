using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")]
public class AdminOnlyModel : PageModel
{
    private readonly ChartOfAccountService _service;

    public AdminOnlyModel(ChartOfAccountService service)
    {
        _service = service;
    }

    public List<ChartOfAccount> AccountsTree { get; set; }
    [BindProperty] public ChartOfAccount FormModel { get; set; }
    public SelectList ParentOptions { get; set; }

    public async Task OnGetAsync()
    {
        var flatList = await _service.GetAllAsync();
        AccountsTree = flatList;

        ParentOptions = new SelectList(
            flatList.Select(a => new { a.Id, Display = $"{a.Code} - {a.Name}" }),
            "Id", "Display");
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        await _service.ManageAsync("CREATE", FormModel);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _service.ManageAsync("DELETE", new ChartOfAccount { Id = id });
        return RedirectToPage();
    }
}
