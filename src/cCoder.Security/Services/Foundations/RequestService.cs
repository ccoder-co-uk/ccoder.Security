// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Requests;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class RequestService(IHttpRequestBroker requestBroker)
    : IRequestService
{
    public string GetHeader(string key) =>
        TryCatch(operation: () =>
        {
            ValidateHeaderOnGet(key: key);

            return requestBroker.Header(key: key);
        });
}