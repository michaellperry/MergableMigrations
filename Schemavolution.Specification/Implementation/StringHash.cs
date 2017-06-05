using Org.BouncyCastle.Crypto.Digests;
using System.Numerics;
using System.Text;

namespace Schemavolution.Specification.Implementation
{
    static class HashExtensions
    {
        public static BigInteger Sha256Hash(this string str)
        {
            if (str == null)
                return BigInteger.Zero;

            var digest = new Sha256Digest();
            byte[] stringBytes = Encoding.UTF8.GetBytes(str);
            digest.BlockUpdate(stringBytes, 0, stringBytes.Length);
            digest.Finish();
            byte[] bytes = new byte[digest.GetDigestSize()];
            digest.DoFinal(bytes, 0);
            return new BigInteger(bytes);
        }

        public static BigInteger Concatenate(this BigInteger start, params BigInteger[] rest)
        {
            var digest = new Sha256Digest();
            byte[] startBytes = start.ToByteArray();
            digest.BlockUpdate(startBytes, 0, startBytes.Length);
            foreach (var b in rest)
            {
                byte[] blockBytes = b.ToByteArray();
                digest.BlockUpdate(blockBytes, 0, blockBytes.Length);
            }

            digest.Finish();
            byte[] bytes = new byte[digest.GetDigestSize()];
            digest.DoFinal(bytes, 0);
            return new BigInteger(bytes);
        }
    }
}
