using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public class RoleSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public RoleSeeder(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        string[] roles = { "Admin", "Accountant", "Viewer" };
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }

        await SeedUserAsync("admin@example.com", "Admin123!", "Admin");
        await SeedUserAsync("accountant@example.com", "Accountant123!", "Accountant");
        await SeedUserAsync("viewer@example.com", "Viewer123!", "Viewer");
    }

    private async Task SeedUserAsync(string email, string password, string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return;
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            await _userManager.AddToRoleAsync(user, role);
        }
    }
}
