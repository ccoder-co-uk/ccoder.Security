// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Exceptions;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class CurrentUserAggregationService
{
    private static T TryCatch<T>(Func<T> operation)
    {
        try
        {
            return operation();
        }
        catch (ArgumentException innerException)
        {
            throw new SecurityAggregationValidationException(innerException: innerException);
        }
        catch (SecurityProcessingValidationException innerException)
        {
            throw new SecurityAggregationValidationException(innerException: innerException);
        }
        catch (SecurityProcessingDependencyException innerException)
        {
            throw new SecurityAggregationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new SecurityAggregationServiceException(innerException: innerException);
        }
    }
}