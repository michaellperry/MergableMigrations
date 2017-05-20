using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Text;

namespace MergableMigrations.Specification.Implementation
{
    static class StringHashExtensions
    {
        public static int Sha356Hash(this string str)
        {
            var digest = new Sha256Digest();
            byte[] stringBytes = Encoding.UTF8.GetBytes(str);
            digest.BlockUpdate(stringBytes, 0, stringBytes.Length);
            digest.Finish();
            byte[] bytes = new byte[digest.GetByteLength()];
            digest.DoFinal(bytes, 0);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
