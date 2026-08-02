// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Events;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Events;
using System.Text.Json;

namespace cCoder.Security.Services.Foundations.Events;

internal sealed partial class EventHandlerService(IEventHubBroker eventHubBroker)
    : IEventHandlerService
{
    public void ListenToAllEvents() =>
        TryCatch(operation: () =>
        {
            eventHubBroker.ListenToEvent(
                eventName: "tenant_setup",
                handler: (ITenantManager manager, SetupDetails details) =>
                    manager.SetupAsync(setupDetails: details));

            foreach (SecurityAccountEventKind kind in Enum.GetValues<SecurityAccountEventKind>())
            {
                string eventName = kind.ToEventName();

                eventHubBroker.ListenToEvent<SecurityAccountEvent, IUserEventManager>(
                    eventName: eventName,
                    handler: async (manager, accountEvent) =>
                        await StoreAccountAuditEventAsync(
                            manager: manager,
                            eventName: eventName,
                            accountEvent: accountEvent));
            }
        });

    internal static ValueTask<UserEvent> StoreAccountAuditEventAsync(
        IUserEventManager manager,
        string eventName,
        SecurityAccountEvent accountEvent)
    {
        string subjectUserId = accountEvent.User?.Id;

        UserEvent userEvent = new()
        {
            Id = Guid.NewGuid(),
            EventName = eventName,
            TenantId = accountEvent.Tenant?.Id,
            CreatedBy = accountEvent.ActorUserId ?? subjectUserId,
            Value = JsonSerializer.Serialize(value: new
            {
                Kind = accountEvent.Kind.ToString(),
                SubjectUserId = subjectUserId,
                accountEvent.RequestDomain,
                accountEvent.Culture
            })
        };

        return manager.AddUserEventAsync(userEvent: userEvent);
    }
}