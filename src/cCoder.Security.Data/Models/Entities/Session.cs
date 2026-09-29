// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace cCoder.Security.Models.Entities;

public class Session
{
    public string Id { get; set; }
    public string Value { get; set; }
    public DateTimeOffset ExpiresAtTime { get; set; }
    public long SlidingExpirationInSeconds { get; set; }
    public DateTimeOffset AbsoluteExpiration { get; set; }
    public virtual ICollection<UserEvent> UserEvents { get; set; }
}