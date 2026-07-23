// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Exceptions;

namespace cCoder.Security.Services.Orchestrations;

internal sealed partial class TenantRelationsOrchestrationService
{
    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new SecurityOrchestrationValidationException(innerException: innerException);
        }
        catch (SecurityProcessingValidationException innerException)
        {
            throw new SecurityOrchestrationValidationException(innerException: innerException);
        }
        catch (SecurityProcessingDependencyException innerException)
        {
            throw new SecurityOrchestrationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new SecurityOrchestrationServiceException(innerException: innerException);
        }
    }
}