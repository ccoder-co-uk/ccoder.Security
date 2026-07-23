// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Exceptions;

namespace cCoder.Security.Services.Processings;

internal sealed partial class AccountEventProcessingService
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
        catch (SecurityValidationException innerException)
        {
            throw new SecurityProcessingValidationException(innerException: innerException);
        }
        catch (SecurityDependencyException innerException)
        {
            throw new SecurityProcessingDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new SecurityProcessingServiceException(innerException: innerException);
        }
    }
}