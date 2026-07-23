// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Brokers.Events;

internal interface IEventHubBroker
{
    void ListenToEvent<T, TService>(string eventName, Func<TService, T, ValueTask> handler);
}