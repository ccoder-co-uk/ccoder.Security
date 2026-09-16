// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Utility;
using cCoder.Security.Services.Processings.Interfaces;
using Microsoft.Extensions.Hosting;

namespace cCoder.Security.Exposures.HostedServices;

internal sealed class TokenCleaner(
    IServiceScopeBroker serviceScopeBroker)
    : BackgroundService, ITokenCleaner
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        serviceScopeBroker.ExecuteInScopeAsync<ITokenProcessingService>(
            operation: tokenProcessingService =>
                tokenProcessingService.ExecuteCleanupAsync(
                    cancellationToken: stoppingToken))
        .AsTask();
}