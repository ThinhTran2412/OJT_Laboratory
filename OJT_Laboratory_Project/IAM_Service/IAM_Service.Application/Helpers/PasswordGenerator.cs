namespace IAM_Service.Application.Helpers
{
    /// <summary>
    /// create method Generate password
    /// </summary>
    public static class PasswordGenerator
    {
        /// <summary>
        /// Generates the specified length.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string Generate(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}