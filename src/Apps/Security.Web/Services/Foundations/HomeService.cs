// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Security.Web.Brokers;

namespace Security.Web.Services.Foundations;

internal sealed partial class HomeService(
    IHomeEnvironmentBroker homeEnvironmentBroker)
    : IHomeService
{
    public string GetIndexPath() =>
        TryCatch(operation: () =>
            homeEnvironmentBroker.GetIndexPath());
}