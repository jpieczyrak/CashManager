using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CashManager.Logic.Extensions
{
    public static class StringExtensions
    {
        public static string Encrypt(this string input)
        {
            var crypto = new SHA512CryptoServiceProvider();

            var buffer = Encoding.ASCII.GetBytes(input);
            for (int i = 0; i < 262144; i++) buffer = crypto.ComputeHash(buffer);

            return Encoding.ASCII.GetString(buffer);
        }

        public static string WildCardToRegex(this string input)
        {
            return $"^{Regex.Escape(input).Replace("\\?", ".").Replace("\\*", ".*")}$";
        }
    }
}