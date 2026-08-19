using System.ComponentModel.DataAnnotations;

namespace Glue.API.Models.Dtos.User;

public class RefreshTokenRequestDto
{
    public required string RefreshToken { get; set; }
}