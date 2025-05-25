using System.IO.Compression;

namespace MiddleWare.ResponseCompresison
{
    public class ResponseCompressionOptions
    {
        public List<string> MimeTypes { get; set; } = new List<string>
        {

            "text/html",
            "text/css",
            "application/javascript",
            "application/json",
            "application/xml",
            "image/svg+xml",
            "application/xhtml+xml",
            "application/x-javascript",
            "text/plain"
        };
        public bool EnableCompression { get; set; } = true;

        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Fastest;


    }
}
