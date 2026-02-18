using System.ComponentModel.DataAnnotations;

namespace PersonApi.Models;

public class City
{
    public int Id { get; set; }

    [Required(ErrorMessage = "City name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "City name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Country must be between 1 and 100 characters")]
    public string Country { get; set; } = string.Empty;
}
