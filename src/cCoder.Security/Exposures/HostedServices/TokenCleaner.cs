using cCoder.Security.Services.Foundations.Interfaces;
using Microsoft.Extensions.Hosting;

namespace cCoder.Security.Exposures.HostedServices;

internal sealed class TokenCleaner(ITokenService tokenService)
    : BackgroundService, ITokenCleaner
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (int.TryParse(Environment.GetEnvironmentVariable("MIGRATING"), out int result) && result == 1)
            return;

        await tokenService.DeleteExpiredAsync(stoppingToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            await tokenService.DeleteExpiredAsync(stoppingToken);
    }
}
