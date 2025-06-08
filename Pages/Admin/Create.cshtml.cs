using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace MiniAccountProject.Pages.Admin
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _config;

        public CreateModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public VoucherInputModel Voucher { get; set; } = new VoucherInputModel();

        public List<Account> Accounts { get; set; }

        public void OnGet()
        {
            // Load accounts for dropdown (fake for now)
            Accounts = new List<Account>
            {
                new Account { Id = 1, Name = "Cash" },
                new Account { Id = 2, Name = "Bank" },
                new Account { Id = 3, Name = "Receivable" }
            };
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var table = new DataTable();
            table.Columns.Add("AccountId", typeof(int));
            table.Columns.Add("Debit", typeof(decimal));
            table.Columns.Add("Credit", typeof(decimal));

            foreach (var entry in Voucher.Entries)
            {
                table.Rows.Add(entry.AccountId, entry.Debit, entry.Credit);
            }

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SaveVoucher", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Date", Voucher.Date);
            cmd.Parameters.AddWithValue("@ReferenceNo", Voucher.ReferenceNo);
            cmd.Parameters.AddWithValue("@VoucherType", Voucher.VoucherType);

            var tvpParam = cmd.Parameters.AddWithValue("@Entries", table);
            tvpParam.SqlDbType = SqlDbType.Structured;
            tvpParam.TypeName = "dbo.VoucherEntryType";

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            TempData["Message"] = "Voucher saved using TVP!";
            return RedirectToPage("/Admin/AdminOnly");
        }


        public class VoucherInputModel
        {
            public DateTime Date { get; set; } = DateTime.Today;
            public string ReferenceNo { get; set; }
            public string VoucherType { get; set; } // Journal, Payment, Receipt
            public List<EntryLine> Entries { get; set; } = new List<EntryLine>();
        }

        public class EntryLine
        {
            public int AccountId { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
        }

        public class Account
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
