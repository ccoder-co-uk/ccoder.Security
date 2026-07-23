// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Security.Web.Exposures;

internal sealed class HomeManager(
    IWebHostEnvironment environment)
        : IHomeManager
{
    public string GetIndexPath() =>
        Path.Combine(
            path1: environment.WebRootPath,
            path2: "index.html");
}