using System.Data.SqlClient;
using System.Data;

public class ChartOfAccountService
{
    private readonly string _connectionString;

    public ChartOfAccountService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<ChartOfAccount>> GetAllAsync()
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SELECT * FROM ChartOfAccounts", conn);
        var accounts = new List<ChartOfAccount>();

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            accounts.Add(new ChartOfAccount
            {
                Id = (int)reader["Id"],
                Name = reader["Name"].ToString(),
                Code = reader["Code"].ToString(),
                ParentId = reader["ParentId"] as int?
            });
        }

        return BuildTree(accounts);
    }

    private List<ChartOfAccount> BuildTree(List<ChartOfAccount> list)
    {
        var lookup = list.ToDictionary(x => x.Id);
        var roots = new List<ChartOfAccount>();

        foreach (var item in list)
        {
            if (item.ParentId.HasValue && lookup.ContainsKey(item.ParentId.Value))
            {
                lookup[item.ParentId.Value].Children.Add(item);
            }
            else
            {
                roots.Add(item);
            }
        }

        return roots;
    }

    public async Task ManageAsync(string action, ChartOfAccount acc)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_ManageChartOfAccounts", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Action", action);
        cmd.Parameters.AddWithValue("@Id", (object?)acc.Id ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Name", (object?)acc.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Code", (object?)acc.Code ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ParentId", (object?)acc.ParentId ?? DBNull.Value);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
