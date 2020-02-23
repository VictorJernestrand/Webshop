using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Webshop.Data
{
    public static class PasswordHandler
    {
        public static string GenerateHash(this string str)
        {
            StringBuilder builder = new StringBuilder();

            // Initialize a SHA256 object
            using (SHA256 sha256 = SHA256.Create())
            {
                // Hash string into a byte[] array
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));

                // Convert the byte array to a string
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
