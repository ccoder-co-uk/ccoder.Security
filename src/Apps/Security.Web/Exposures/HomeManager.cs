// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Security.Web.Services.Foundations;

namespace Security.Web.Exposures;

internal sealed class HomeManager(IHomeService homeService)
    : IHomeManager
{
    public string GetIndexPath() =>
        homeService.GetIndexPath();
}