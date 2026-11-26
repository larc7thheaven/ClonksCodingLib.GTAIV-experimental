using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace CCL.GTAIV
{
    internal static class InternalHelper
    {

        // One Random per thread, each seeded from a shared generator. Creating "new Random()" per call seeds from the
        // tick count, so calls within the same ~15 ms return the same values; Random itself is not thread-safe.
        private static readonly Random seedSource = new Random();
        private static readonly ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(() =>
        {
            lock (seedSource)
                return new Random(seedSource.Next());
        });

        public static Random Random => threadRandom.Value;

        public static string CompressString(string uncompressedString)
        {
            try
            {
                byte[] compressedBytes;

                using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
                {
                    using (var compressedStream = new MemoryStream())
                    {
                        using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Optimal, true))
                            uncompressedStream.CopyTo(compressorStream);

                        compressedBytes = compressedStream.ToArray();
                    }
                }

                return Convert.ToBase64String(compressedBytes);
            }
            catch (Exception){}

            return string.Empty;
        }
        public static string DecompressString(string compressedString)
        {
            try
            {
                byte[] decompressedBytes;

                var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

                using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        decompressorStream.CopyTo(decompressedStream);

                        decompressedBytes = decompressedStream.ToArray();
                    }
                }

                return Encoding.UTF8.GetString(decompressedBytes);
            }
            catch (Exception){}

            return string.Empty;
        }

    }
}
