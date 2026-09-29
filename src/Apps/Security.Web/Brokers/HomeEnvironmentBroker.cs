// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.IO;
using System;

using Microsoft.AspNetCore.Hosting;

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