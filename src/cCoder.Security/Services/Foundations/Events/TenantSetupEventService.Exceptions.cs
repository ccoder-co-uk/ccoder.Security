// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Security.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Security.Services.Foundations.Events;

internal sealed partial class TenantSetupEventService
{
    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
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