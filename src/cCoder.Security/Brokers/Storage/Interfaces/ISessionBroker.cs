using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;
internal interface ISessionBroker
{
    ValueTask<Session> AddSessionAsync(Session Session);
    ValueTask DeleteSessionAsync(Session Session);
    IQueryable<Session> GetAllSessions();
    ValueTask<Session> UpdateSessionAsync(Session Session);
}

