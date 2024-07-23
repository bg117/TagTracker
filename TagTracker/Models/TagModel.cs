using System.ComponentModel.DataAnnotations;

namespace TagTracker.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }

    public string Uid      { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Lrn      { get; set; } = string.Empty;
}
