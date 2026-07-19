using System.Security.Cryptography;

namespace Albatros.Security
{
    // Simple PBKDF2 password hashing so we never store or compare plain-text passwords.
    // No extra NuGet packages needed -- everything here is built into .NET.
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(key)}";
        }

        public static bool Verify(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedKey = Convert.FromBase64String(parts[1]);
            byte[] actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

            return CryptographicOperations.FixedTimeEquals(expectedKey, actualKey);
        }
    }
}
