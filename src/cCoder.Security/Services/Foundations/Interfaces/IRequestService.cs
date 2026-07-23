// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface IRequestService
{
    string GetHeader(string key);
}