// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class SSOPrivilegeService
{
    private static T TryCatch<T>(Func<T> operation)
    {
        try
        {
            return operation();
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