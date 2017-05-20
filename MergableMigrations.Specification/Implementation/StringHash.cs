using System.Security.Cryptography;
using System.Text;

namespace MergableMigrations.Specification.Implementation
{
    static class StringHashExtensions
    {
        private static SHA256 Sha256Alrorithm = SHA256.Create();

        public static int Sha356Hash(this string str)
        {
            var bytes = Sha256Alrorithm.ComputeHash(Encoding.UTF8.GetBytes(str));
            return System.BitConverter.ToInt32(bytes, 0);
        }
    }
}
