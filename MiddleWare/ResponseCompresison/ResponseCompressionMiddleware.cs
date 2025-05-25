
using System.IO.Compression;

namespace MiddleWare.ResponseCompresison
{
    public class ResponseCompressionMiddleware
    {
        private RequestDelegate next;
        private ResponseCompressionOptions CompressionOptions;
        private readonly ILogger<ResponseCompressionMiddleware> Logger;
        public ResponseCompressionMiddleware(RequestDelegate _next, ResponseCompressionOptions options,
            ILogger<ResponseCompressionMiddleware> logger)
        {
            next = _next;
            CompressionOptions = options;
            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Logger.LogInformation("响应压缩中间件==start==");
            var originalBodyStream = context.Response.Body;

            // 获取接受的压缩方法
            var acceptEncoding = context.Request.Headers["Accept-Encoding"].ToString().ToLowerInvariant();
            // 是否支持压缩
            if (ShouldCompress(context, acceptEncoding))
            {
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                await next(context);

                if (memoryStream.Length > 0)
                {
                    memoryStream.Position = 0;
                    if (acceptEncoding.Contains("gzip"))
                    {
                        context.Response.Headers.Add("Content-Encoding", "gzip");
                        await CompressStreamGzip(memoryStream, originalBodyStream, CompressionOptions.CompressionLevel);
                    }
                    else if (acceptEncoding.Contains("deflate"))
                    {
                        context.Response.Headers.Add("Content-Encoding", "deflate");
                        await CompressStreamDeflate(memoryStream, originalBodyStream, CompressionOptions.CompressionLevel);
                    }
                    else
                    {
                        memoryStream.Position = 0;
                        await memoryStream.CopyToAsync(originalBodyStream);
                    }
                }

            }
            else
            {
                await next(context);

            }
        }

        private async Task CompressStreamDeflate(MemoryStream source, Stream destination, CompressionLevel compressionLevel)
        {
            using var deflateStream = new DeflateStream(destination, compressionLevel, true);
            await source.CopyToAsync(deflateStream);
        }

        private async Task CompressStreamGzip(MemoryStream source, Stream destination, CompressionLevel compressionLevel)
        {
            using var gzipStream = new GZipStream(destination, compressionLevel, true);
            await source.CopyToAsync(gzipStream);
        }

        private bool ShouldCompress(HttpContext context, string? acceptEncoding)
        {
            var compressionTypeList = new List<string>() { "gzip", "deflate" };
            // 不支持压缩的情况
            if (string.IsNullOrEmpty(acceptEncoding) || !compressionTypeList.Contains(acceptEncoding))
            {
                return false;
            }
            // Https请求不启用HTTPS压缩的情况
            if (context.Request.IsHttps && !CompressionOptions.EnableCompression)
            {
                return false;
            }
            // 检查内存类型是否应该被检查
            var contentType = context.Response.ContentType;
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }
            return CompressionOptions.MimeTypes.Any(mimeType => contentType.Contains(mimeType, StringComparison.OrdinalIgnoreCase));
        }
    }
}