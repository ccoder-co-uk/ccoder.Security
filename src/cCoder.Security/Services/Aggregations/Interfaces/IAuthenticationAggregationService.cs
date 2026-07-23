// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Aggregations.Interfaces;

public interface IAuthenticationAggregationService
{
    ValueTask<Token> IssueTokenAsync(string userId, TokenUse tokenUse);

    ValueTask<Token> LoginAsync(string username, string password);

    ValueTask LogoutAsync();

    ValueTask ChangePasswordAsync(string username, string oldPassword, string newPassword);

    ValueTask<Token> ForgotPasswordAsync(string email);

    ValueTask ConfirmForgotPasswordAsync(string tokenId, string userId, string newPassword, string confirmNewPassword);
}