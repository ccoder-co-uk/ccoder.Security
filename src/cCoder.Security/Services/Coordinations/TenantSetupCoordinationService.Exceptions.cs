// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Exceptions;

namespace cCoder.Security.Services.Coordinations;

internal sealed partial class TenantSetupCoordinationService
{
    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new SecurityProcessingValidationException(innerException: innerException);
        }
        catch (SecurityProcessingValidationException innerException)
        {
            throw new SecurityProcessingValidationException(innerException: innerException);
        }
        catch (SecurityProcessingDependencyException innerException)
        {
            throw new SecurityProcessingDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new SecurityProcessingServiceException(innerException: innerException);
        }
    }
}