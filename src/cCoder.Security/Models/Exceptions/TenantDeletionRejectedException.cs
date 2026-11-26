// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Security.Models.Exceptions;

/// <summary>A tenant deletion veto with a message safe for the caller.</summary>
public sealed class TenantDeletionRejectedException(TenantDeletionRejection reason)
    : ArgumentException(message: "The tenant still owns dependent resources.")
{
    public TenantDeletionRejection Reason { get; } = reason;
}

public enum TenantDeletionRejection
{
    DependentResource,
    LinkedApplications,
    LinkedBucketTree
}
