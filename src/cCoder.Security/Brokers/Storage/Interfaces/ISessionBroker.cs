// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISessionBroker
{
    ValueTask<Session> InsertSessionAsync(Session newSession);
    ValueTask DeleteSessionAsync(Session deletedSession);
    IQueryable<Session> SelectAllSessions();
    ValueTask<Session> UpdateSessionAsync(Session updatedSession);
}