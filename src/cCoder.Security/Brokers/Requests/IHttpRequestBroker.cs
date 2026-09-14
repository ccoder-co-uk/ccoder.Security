// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Exposures;

namespace cCoder.Security.Brokers.Requests;

internal interface IHttpRequestBroker : IUtilityBroker
{
    bool HasHeader(string headerValue);
    string Header(string key);
    string RequestHost();
}