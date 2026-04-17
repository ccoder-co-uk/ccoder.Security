using cCoder.Security.Services.Foundations.Events;

namespace cCoder.Security.Exposures.EventHandlers;

internal class SecurityEventHandlers(IEventHandlerService eventHandlerService) : ISecurityEventHandlers
{
    public void ListenToAllEvents() => eventHandlerService.ListenToAllEvents();
}
