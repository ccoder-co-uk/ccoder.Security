// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Exceptions;
using Xunit;

namespace cCoder.Security.Tests.Aggregations;

public partial class AuthenticationAggregationServiceTests
{
    [Fact]
    public async Task LoginThrowsSecurityExceptionIfNotValidLogin()
    {
        // Given
        string username = RandomString();
        string password = RandomString();

        // When
        // Then
        await Assert.ThrowsAsync<SecurityAggregationServiceException>(
            testCode: async () =>
                await authenticationAggregationService.LoginAsync(
                    username: username,
                    password: password));
    }
}