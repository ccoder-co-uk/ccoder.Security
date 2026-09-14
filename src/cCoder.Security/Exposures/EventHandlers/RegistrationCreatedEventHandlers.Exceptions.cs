// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Security.Exposures.EventHandlers;

internal sealed partial class RegistrationCreatedEventHandlers
{
    private void TryCatch(Action operation)
    {
        try
        {
            operation();
        }
        catch (ValidationException innerException)
        {
            loggingBroker.LogException(exception: innerException);

            throw new SecurityValidationException(
                innerException: innerException);
        }
        catch (InvalidOperationException innerException)
        {
            loggingBroker.LogException(exception: innerException);

            throw new SecurityDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            loggingBroker.LogException(exception: innerException);

            throw new SecurityServiceException(
                innerException: innerException);
        }
    }
}