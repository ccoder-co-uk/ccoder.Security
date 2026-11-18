// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Security;

using cCoder.Security.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Aggregations;

public partial class TenantAggregationServiceTests
{
    [Fact]
    public void GetAllTenants_WhenAuthorizationRejectsRequest_ShouldThrowValidationException()
    {
        // Given
        authorizationProcessingServiceMock
            .Setup(expression: service => service.EnsureUserHasPrivilege(
                privilege: "tenant_read"))
            .Throws(exception: new SecurityException(message: "access denied"));

        // When
        Action action = () => tenantAggregationService.GetAllTenants();

        // Then
        action.Should()
            .Throw<SecurityAggregationValidationException>();
    }
}