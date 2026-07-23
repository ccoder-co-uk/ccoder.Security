// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Security.Web.Exposures.Setup;

namespace Security.Web.Exposures;

internal sealed class UIBaselineManager : IUIBaselineManager
{
    public object GetPackages() =>
        UIBaseline.Packages;
}