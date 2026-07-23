// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Exceptions;

namespace cCoder.Security.Services.Managements;

internal sealed partial class TenantSetupManagementService
{
    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new SecurityManagementValidationException(innerException: innerException);
        }
        catch (SecurityAggregationValidationException innerException)
        {
            throw new SecurityManagementValidationException(innerException: innerException);
        }
        catch (SecurityOrchestrationValidationException innerException)
        {
            throw new SecurityManagementValidationException(innerException: innerException);
        }
        catch (SecurityAggregationDependencyException innerException)
        {
            throw new SecurityManagementDependencyException(innerException: innerException);
        }
        catch (SecurityOrchestrationDependencyException innerException)
        {
            throw new SecurityManagementDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new SecurityManagementServiceException(innerException: innerException);
        }
    }
}