using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TagTracker.Models;

public partial class Tag : ObservableObject
{
    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _lrn = string.Empty;

    [ObservableProperty]
    private string _uid = string.Empty;

    [Key]
    public int Id { get; set; }
}
