using MailScheduler.Infrastructure.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Infrastructure.HealthChecks
{
    public class SmtpHealthCheck : IHealthCheck
    {
        private readonly SmtpSettings _smtp;

        public SmtpHealthCheck(IOptions<SmtpSettings> options)
        {
            _smtp = options.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(_smtp.Host, _smtp.Port);
                return HealthCheckResult.Healthy("SMTP reachable");
            }
            catch (SocketException ex)
            {
                return HealthCheckResult.Unhealthy("SMTP unreachable", ex);
            }
        }
    }
}
