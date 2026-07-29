// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures.HostedServices;
using cCoder.Security.Models;
using cCoder.Security.Services.Foundations.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cCoder.Security.Dependencies.HostedServices;

internal sealed class TokenCleaner(
    IServiceScopeFactory serviceScopeFactory,
    SecurityConfiguration securityConfiguration)
    : BackgroundService, ITokenCleaner
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (securityConfiguration.IsMigrating)
        {
            return;
        }

        using IServiceScope scope =
            serviceScopeFactory.CreateScope();

        ITokenService tokenService =
            scope.ServiceProvider.GetRequiredService<ITokenService>();

        await tokenService.DeleteExpiredAsync(
            cancellationToken: stoppingToken);

        using PeriodicTimer timer = new(
            period: TimeSpan.FromMinutes(minutes: 1));

        while (!stoppingToken.IsCancellationRequested
            && await timer.WaitForNextTickAsync(
                cancellationToken: stoppingToken))
        {
            await tokenService.DeleteExpiredAsync(
                cancellationToken: stoppingToken);
        }
    }
}