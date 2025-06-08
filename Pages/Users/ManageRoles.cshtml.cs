using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Admin")]
public class ManageRolesModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ManageRolesModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public List<UserWithRoles> Users { get; set; }

    public class UserWithRoles
    {
        public IdentityUser User { get; set; }
        public IList<string> Roles { get; set; }
    }

    public async Task OnGetAsync()
    {
        Users = new List<UserWithRoles>();
        var users = _userManager.Users.ToList();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            Users.Add(new UserWithRoles { User = user, Roles = roles });
        }
    }

    public async Task<IActionResult> OnPostAsync(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
        }

        await _userManager.AddToRoleAsync(user, newRole);

        return RedirectToPage();
    }
}
