// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISessionBroker
{
    ValueTask<Session> InsertSessionAsync(Session Session);
    ValueTask DeleteSessionAsync(Session Session);
    IQueryable<Session> SelectAllSessions();
    ValueTask<Session> UpdateSessionAsync(Session Session);
}