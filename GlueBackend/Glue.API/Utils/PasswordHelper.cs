namespace Glue.API.Utils;

public static class PasswordUtil
{
    #region -- HashPassword()
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(11));
    }
    #endregion

    #region -- VerifyPassword()
    public static bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (Exception)
        {

            return false;
        }
    }
    #endregion
}
