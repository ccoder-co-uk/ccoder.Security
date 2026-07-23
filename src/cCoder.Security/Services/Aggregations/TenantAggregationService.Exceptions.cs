// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Exceptions;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class TenantAggregationService
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

    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
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

    private static async ValueTask<T> TryCatch<T>(Func<ValueTask<T>> operation)
    {
        try
        {
            return await operation();
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