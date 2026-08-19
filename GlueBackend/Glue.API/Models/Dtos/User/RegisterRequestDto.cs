using System.ComponentModel.DataAnnotations;

namespace Glue.API.Models.Dtos.User;

public class RegisterRequestDto
{
    [StringLength(128)]
    public required string UserName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    [MinLength(6)]
    public required string Password { get; set; }

    [Compare("Password")]
    public required string ConfirmPassword { get; set; }
}
