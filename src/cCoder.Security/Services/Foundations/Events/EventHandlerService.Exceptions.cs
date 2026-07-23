// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Security.Services.Foundations.Events;

internal sealed partial class EventHandlerService
{
    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
        }
        catch (ValidationException innerException)
        {
            throw new SecurityValidationException(innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            throw new SecurityDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new SecurityServiceException(innerException: innerException);
        }
    }
}