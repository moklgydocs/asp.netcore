namespace MiddleWare.SecurityHeadersMiddleware
{
    public class SecurityHeadersOptions
    {
        public bool UseHsts { get; set; } = true;

        public bool UseXssProtection { get; set; } = true;

        public bool UseContentTypeOptions { get; set; } = true;

        public bool UseFrameOptions { get; set; } = true;

        public string ContentSecurityPolicy { get; set; } = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self'; connect-src 'self'; font-src 'self'; frame-src 'self'; object-src 'none'; base-uri 'self'; form-action 'self'; upgrade-insecure-requests; block-all-mixed-content; report-uri /csp-report-endpoint; report-to csp-endpoint;";


    }
}
