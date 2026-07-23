// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class RequestProcessingService(
    IRequestService requestService)
        : IRequestProcessingService
{
    public string GetHeader(string key) =>
        TryCatch(operation: () =>
        {
            ValidateHeaderOnGet(key: key);

            return requestService.GetHeader(key: key);
        });
}