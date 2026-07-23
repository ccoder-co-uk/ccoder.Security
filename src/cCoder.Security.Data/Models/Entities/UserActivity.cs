// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Objects.Entities;

public class UserActivity
{
    public string TenantId { get; set; }
    public string TenantName { get; set; }
    public string TenantDescription { get; set; }
    public string TenantCreatedBy { get; set; }
    public string TenantLastUpdatedBy { get; set; }
    public DateTimeOffset TenantCreatedOn { get; set; }
    public DateTimeOffset TenantLastUpdated { get; set; }

    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public string UserEmail { get; set; }
    public string UserPhoneNumber { get; set; }

    public Guid EventId { get; set; }
    public string EventName { get; set; }
    public string EventValue { get; set; }
    public DateTimeOffset EventCreatedOn { get; set; }

    public string SessionId { get; set; }
    public DateTimeOffset SessionExpiresAtTime { get; set; }
}