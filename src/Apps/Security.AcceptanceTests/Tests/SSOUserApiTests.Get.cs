using FluentAssertions;
using Security.AcceptanceTests.Tests.Models;
using Security.Objects.DTOs;
using Security.Objects.Entities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Security.AcceptanceTests.Tests
{
    public partial class SSOUserApiTests
	{
		[Fact]
		public async void ShouldGetAllSSOUsersAsync()
		{
			//given
			RegisterUser[] randomUsers = RandomRegisterUsers();
            List <SSOUser> expectedSSOUsers = new List<SSOUser>();

			foreach (var registerUser in randomUsers)
			{
                RegistrationResult result = await 
					registerApiClient.RegisterAsync(registerUser);

                expectedSSOUsers.Add(result.User);
			}

			//when
			IEnumerable<SSOUser> actualSSOUsers = 
				await ssoUserApiClient.GetAllSSOUsersAsync();

			//then
			foreach (var expectedUser in expectedSSOUsers)
			{
				var actualUser = actualSSOUsers.FirstOrDefault(u => u.Id == expectedUser.Id);
                actualUser.Should().NotBeNull();
				actualUser.Should().BeEquivalentTo(expectedUser);
			}

			foreach (var ssoUser in expectedSSOUsers)
				await registerApiClient.TearDown(ssoUser.Id);
		}
	}
}