using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ChartOfAccount
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Code { get; set; }

    public int? ParentId { get; set; }

    public List<ChartOfAccount> Children { get; set; } = new();
}
