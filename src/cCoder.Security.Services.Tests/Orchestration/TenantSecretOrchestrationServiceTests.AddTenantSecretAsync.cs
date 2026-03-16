using System.ComponentModel.DataAnnotations;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class TenantSecretOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldAddTenantSecretAsync()
    {
        TenantSecret inputTenantSecret = RandomTenantSecret();

        tenantSecretProcessingServiceMock.Setup(service =>
            service.GetAllTenantSecrets())
            .Returns(Array.Empty<TenantSecret>().AsQueryable());

        tenantSecretProcessingServiceMock.Setup(service =>
            service.AddTenantSecretAsync(inputTenantSecret))
            .ReturnsAsync(inputTenantSecret);

        TenantSecret actualTenantSecret =
            await tenantSecretOrchestrationService.AddTenantSecretAsync(inputTenantSecret);

        actualTenantSecret.Value.Should().BeNull();

        authBrokerMock.Verify(broker =>
            broker.UserHasPrivilege("tenantsecret_create", inputTenantSecret.TenantId), Times.Once());
        tenantSecretProcessingServiceMock.Verify(service =>
            service.GetAllTenantSecrets(), Times.Once());
        tenantSecretProcessingServiceMock.Verify(service =>
            service.AddTenantSecretAsync(inputTenantSecret), Times.Once());
        authBrokerMock.VerifyNoOtherCalls();
        tenantSecretProcessingServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldRejectDuplicateTenantSecretAsync()
    {
        TenantSecret inputTenantSecret = RandomTenantSecret();

        tenantSecretProcessingServiceMock.Setup(service =>
            service.GetAllTenantSecrets())
            .Returns(new[] { inputTenantSecret }.AsQueryable());

        Func<Task> action = async () =>
            await tenantSecretOrchestrationService.AddTenantSecretAsync(inputTenantSecret);

        await action.Should().ThrowAsync<ValidationException>();

        authBrokerMock.Verify(broker =>
            broker.UserHasPrivilege("tenantsecret_create", inputTenantSecret.TenantId), Times.Once());
        tenantSecretProcessingServiceMock.Verify(service =>
            service.GetAllTenantSecrets(), Times.Once());
        authBrokerMock.VerifyNoOtherCalls();
        tenantSecretProcessingServiceMock.VerifyNoOtherCalls();
    }
}
