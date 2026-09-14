// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Events;

namespace cCoder.Security.Services.Processings.Interfaces;

internal interface IAccountAuditUserEventProcessingService
{
    ValueTask StoreSecurityAccountEventAuditAsync(
        string eventName,
        SecurityAccountEvent securityAccountEvent);
}