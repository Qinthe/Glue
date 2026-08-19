namespace Glue.API.Database.Entities.GlueUser;

public class GlueTokenBlacklist : BaseEntity
{
    public required string TokenJti { get; set; }
    public required string UserId { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
