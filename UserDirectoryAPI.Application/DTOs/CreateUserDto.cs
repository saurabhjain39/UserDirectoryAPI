using System.ComponentModel.DataAnnotations;

namespace UserDirectoryAPI.Application.DTOs;

public class CreateUserDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = default!;

    [Required]
    [Range(0, 120)]
    public int Age { get; set; }

    [Required]
    public string City { get; set; } = default!;

    [Required]
    public string State { get; set; } = default!;

    [Required]
    [StringLength(10, MinimumLength = 4)]
    public string Pincode { get; set; } = default!;
}
