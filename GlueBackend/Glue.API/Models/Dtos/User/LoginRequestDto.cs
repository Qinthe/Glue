using System.ComponentModel.DataAnnotations;

namespace Glue.API.Models.Dtos.User;

public class LoginRequestDto
{
    [EmailAddress]
    public required string Email { get; set; }

    [MinLength(6)]
    public required string Password { get; set; }
}
