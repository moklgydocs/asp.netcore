namespace MiddleWare.ResponseCompresison
{
    public static class ResponseCompressionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCompression(this IApplicationBuilder app, Action<ResponseCompressionOptions> compressionOptions = null)
        {
            var responseCompression = new ResponseCompressionOptions();
            compressionOptions?.Invoke(responseCompression);
            app.UseMiddleware<ResponseCompressionMiddleware>(responseCompression);
            return app;
        }

    }
}
