using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Security.AcceptanceTests.Tests.Models;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class TenantApiTests
{
    [Fact]
    public async void ShouldRegisterAccountAsync()
    {
        //given
        RegisterUser inputRegisterUser = RandomRegisterUser();
        SSOUser expectedSSOUser = new()
        {
            AccessFailedCount = 0,
            DisplayName = inputRegisterUser.DisplayName,
            Email = inputRegisterUser.Email,
            EmailConfirmed = false,
            LockoutEnabled = false,
            LockoutEndDateUtc = null,
            PhoneNumber = inputRegisterUser.PhoneNumber,
            PhoneNumberConfirmed = false,
        };

        //when
        RegistrationResult result = await registerApiClient
            .RegisterAsync(inputRegisterUser);

        SSOUser actualSSOUser = result.User;    

        expectedSSOUser.Id = actualSSOUser.Id;
        expectedSSOUser.PasswordHash = actualSSOUser.PasswordHash;

        //then
        actualSSOUser.Should().BeEquivalentTo(expectedSSOUser);
        await TearDownUserAsync(actualSSOUser.Id);
    }

    [Fact]
    public async void ShouldDeleteTenantAsync()
    {
        // Given.
        Tenant tenant = RandomTenant();


        /*
        // given
        B2BSystem inputSystem = await systemClient.AddSystemAsync(RandomSystem());
        Bucket inputBucket = await bucketClient.AddBucketAsync(RandomBucket());

        BucketSystem inputBucketSystem = await bucketSystemClient.AddBucketSystemAsync(new BucketSystem { BucketId = inputBucket.Id, SystemId = inputSystem.Id });

        // when 
        await bucketSystemClient.DeleteBucketSystemAsync(inputBucket.Id, inputSystem.Id);

        // then
        await bucketClient.DeleteBucketAsync(inputBucket.Id);
        await systemClient.DeleteSystemAsync(inputSystem.Id);
        await Assert.ThrowsAsync<HttpRequestException>(async () => await bucketSystemClient.GetBucketSystemByIdAsync(inputBucket.Id, inputSystem.Id));
        */
    }
}
