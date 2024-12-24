using BCrypt.Net;

public static class PasswordHasher
{
    // Şifreyi hash'leme
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Hash karşılaştırma
    public static bool VerifyPassword(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }
}
