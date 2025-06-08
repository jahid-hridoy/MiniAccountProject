using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace MiniAccountProject.Pages.Permissions
{
    [Authorize(Roles = "Admin")]
    public class ManageModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public ManageModel(RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public List<string> Roles { get; set; } = new();
        public List<ModulePermission> Modules { get; set; } = new();

        [BindProperty]
        public List<PermissionUpdate> Updates { get; set; }

        public class ModulePermission
        {
            public int ModuleId { get; set; }
            public string ModuleName { get; set; }
            public Dictionary<string, bool> RoleAccess { get; set; } = new();
        }

        public class PermissionUpdate
        {
            public string RoleId { get; set; }
            public int ModuleId { get; set; }
            public bool CanAccess { get; set; }
        }

        public async Task OnGetAsync()
        {
            Roles = _roleManager.Roles.Select(r => r.Name).ToList();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            var cmd = new SqlCommand("SELECT Id, Name FROM Modules", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var module = new ModulePermission
                {
                    ModuleId = reader.GetInt32(0),
                    ModuleName = reader.GetString(1)
                };

                foreach (var role in Roles)
                {
                    using var checkCmd = new SqlCommand("sp_HasModuleAccess", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    checkCmd.Parameters.AddWithValue("@RoleName", role);
                    checkCmd.Parameters.AddWithValue("@ModuleName", module.ModuleName);

                    var access = Convert.ToInt32(await checkCmd.ExecuteScalarAsync() ?? 0);
                    module.RoleAccess[role] = access == 1;
                }

                Modules.Add(module);
            }
        }

        public IActionResult OnPost()
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            foreach (var update in Updates)
            {
                using var cmd = new SqlCommand("sp_AssignRoleModuleAccess", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@RoleId", update.RoleId);
                cmd.Parameters.AddWithValue("@ModuleId", update.ModuleId);
                cmd.Parameters.AddWithValue("@CanAccess", update.CanAccess);
                cmd.ExecuteNonQuery();
            }

            return RedirectToPage();
        }
    }
}
