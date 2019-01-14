using System;
using System.Security.Cryptography;
using System.Text;

namespace CashManager.Data.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Generates GUID based on input
        /// </summary>
        /// <returns></returns>
        public static Guid GenerateGuid(this string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                    return new Guid(hash);
                }
            }

            return Guid.NewGuid();
        }
    }
}