using System;
using System.Threading;
using Enterprise.Extensions.HealthChecks;
using Microsoft.AspNetCore.Hosting;

namespace Enterprise.Library.HealthChecks
{
    public static class HealthCheckWebHostExtensions
    {
        private const int DefaultTimeoutSeconds = 300;

        public static void RunWhenHealthy(this IWebHost webHost)
        {
            webHost.RunWhenHealthy(TimeSpan.FromSeconds(DefaultTimeoutSeconds));
        }

        public static void RunWhenHealthy(this IWebHost webHost, TimeSpan timeout)
        {
            var healthChecks = webHost.Services.GetService(typeof(IHealthCheckService)) as IHealthCheckService;

            var loops = 0;
            do
            {
                var checkResult = healthChecks?.CheckHealthAsync().Result;
                if (checkResult?.CheckStatus == CheckStatus.Healthy)
                {
                    webHost.Run();
                    break;
                }

                Thread.Sleep(1000);
                loops++;
            } while (loops < timeout.TotalSeconds);
        }
    }
}