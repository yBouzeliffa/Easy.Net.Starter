namespace Goldweb.Core.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] StreamToByteArray(this Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

    }
}
