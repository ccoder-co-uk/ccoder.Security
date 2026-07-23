// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects;
using cCoder.Security.Services.Foundations.Interfaces;
using Microsoft.Extensions.Hosting;

namespace cCoder.Security.Exposures.HostedServices;

internal sealed class TokenCleaner(
    ITokenService tokenService,
    SecurityConfiguration securityConfiguration)
    : BackgroundService, ITokenCleaner
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (securityConfiguration.IsMigrating)
            return;

        await tokenService.DeleteExpiredAsync(cancellationToken: stoppingToken);

        using PeriodicTimer timer = new(TimeSpan.FromMinutes(minutes: 1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken: stoppingToken))
            await tokenService.DeleteExpiredAsync(cancellationToken: stoppingToken);
    }
}