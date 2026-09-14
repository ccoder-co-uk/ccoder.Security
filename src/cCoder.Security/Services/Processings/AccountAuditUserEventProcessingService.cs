// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Serialization;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Events;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class AccountAuditUserEventProcessingService(
    IUserEventService userEventService,
    ISerializationBroker serializationBroker)
        : IAccountAuditUserEventProcessingService
{
    public ValueTask StoreSecurityAccountEventAuditAsync(
        string eventName,
        SecurityAccountEvent securityAccountEvent) =>
        TryCatch(operation: async () =>
        {
            ValidateSecurityAccountEventOnStore(
                eventName: eventName,
                securityAccountEvent: securityAccountEvent);

            string subjectUserId = securityAccountEvent.User?.Id;

            UserEvent userEvent = new()
            {
                Id = Guid.NewGuid(),
                EventName = eventName,
                TenantId = securityAccountEvent.Tenant?.Id,
                CreatedBy =
                    securityAccountEvent.ActorUserId ?? subjectUserId,
                Value = serializationBroker.Serialize(obj: new
                {
                    Kind = securityAccountEvent.Kind.ToString(),
                    SubjectUserId = subjectUserId,
                    securityAccountEvent.RequestDomain,
                    securityAccountEvent.Culture
                })
            };

            await userEventService.AddUserEventAsync(
                userEvent: userEvent);
        });
}