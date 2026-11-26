// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

namespace cCoder.Security.Exposures;

/// <summary>
/// Handles the in-process <c>tenant_delete</c> event before any tenant data is removed.
/// Throw a <see cref="Models.Exceptions.TenantDeletionRejectedException"/> to veto the deletion.
/// </summary>
public interface ITenantDeletingHandler
{
    ValueTask HandleAsync(string tenantId);
}