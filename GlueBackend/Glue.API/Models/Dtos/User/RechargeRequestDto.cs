using System.ComponentModel.DataAnnotations;

namespace Glue.API.Models.Dtos.User;

public class RechargeRequestDto
{
    [Range(0.01, 100000)]
    public required decimal Amount { get; set; }
}
