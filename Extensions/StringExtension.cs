namespace Easy.Net.Starter.Extensions
{
    public static class StringExtension
    {
        public static string HashPassword(this string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
            return passwordHash;
        }
    }
}
