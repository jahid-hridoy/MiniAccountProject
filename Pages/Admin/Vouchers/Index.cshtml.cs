using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace MiniAccountProject.Pages.Admin.Vouchers
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        public List<VoucherViewModel> Vouchers { get; set; } = new();

        public void OnGet()
        {
            var connStr = _config.GetConnectionString("DefaultConnection");

            using var conn = new SqlConnection(connStr);
            var cmd = new SqlCommand("SELECT * FROM Vouchers ORDER BY VoucherId DESC", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Vouchers.Add(new VoucherViewModel
                {
                    VoucherId = (int)reader["VoucherId"],
                    VoucherDate = ((DateTime)reader["VoucherDate"]).ToShortDateString(),
                    ReferenceNo = reader["ReferenceNo"].ToString(),
                    VoucherType = reader["VoucherType"].ToString()
                });
            }
        }

        public class VoucherViewModel
        {
            public int VoucherId { get; set; }
            public string VoucherDate { get; set; }
            public string ReferenceNo { get; set; }
            public string VoucherType { get; set; }
        }
    }
}
