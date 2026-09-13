// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Security.Web.Brokers;

internal sealed class HomeEnvironmentBroker(
    IWebHostEnvironment environment)
    : IHomeEnvironmentBroker
{
    public string GetIndexPath() =>
        Path.Combine(
            path1: environment.WebRootPath,
            path2: "index.html");
}