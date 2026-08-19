using System.ComponentModel.DataAnnotations;

namespace Glue.API.Models.Dtos.User;

public class ChangePasswordRequestDto
{
    public required string OldPassword { get; set; }

    [MinLength(6)]
    public required string NewPassword { get; set; }

    [Compare("NewPassword")]
    public required string ConfirmNewPassword { get; set; }
}
