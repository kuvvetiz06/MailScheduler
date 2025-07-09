using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MailScheduler.Infrastructure.Authorization
{
    public class HangfireBasicAuthFilter : IDashboardAuthorizationFilter
    {
        private readonly string _user;
        private readonly string _pass;

        public HangfireBasicAuthFilter(string user, string pass)
        {
            _user = user;
            _pass = pass;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var header = httpContext.Request.Headers["Authorization"];
            if (AuthenticationHeaderValue.TryParse(header, out var authHeader) && authHeader.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
            {
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter ?? string.Empty);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                if (credentials.Length == 2 &&
                    credentials[0] == _user &&
                    credentials[1] == _pass)
                {
                    return true;
                }
            }

            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return false;
        }
    }
}
