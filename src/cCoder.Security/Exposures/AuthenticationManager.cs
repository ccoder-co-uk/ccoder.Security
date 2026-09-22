// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class AuthenticationManager(
    IAuthenticationAggregationService authenticationAggregationService)
        : IAuthenticationManager
{
    public ValueTask<Token> IssueTokenAsync(string userId, TokenUse tokenUse) =>
        authenticationAggregationService.IssueTokenAsync(
            userId: userId,
            tokenUse: tokenUse);

    public ValueTask<Token> LoginAsync(string username, string password) =>
        authenticationAggregationService.LoginAsync(
            username: username,
            password: password);

    public ValueTask LogoutAsync() =>
        authenticationAggregationService.LogoutAsync();

    public ValueTask ChangePasswordAsync(
        string username,
        string oldPassword,
        string newPassword) =>
        authenticationAggregationService.ChangePasswordAsync(
            username: username,
            oldPassword: oldPassword,
            newPassword: newPassword);

    public ValueTask ChangeCurrentUserPasswordAsync(
        string oldPassword,
        string newPassword) =>
        authenticationAggregationService.ChangeCurrentUserPasswordAsync(
            oldPassword: oldPassword,
            newPassword: newPassword);

    public ValueTask<Token> ForgotPasswordAsync(string email) =>
        authenticationAggregationService.ForgotPasswordAsync(email: email);

    public ValueTask ConfirmForgotPasswordAsync(
        string tokenId,
        string userId,
        string newPassword,
        string confirmNewPassword) =>
        authenticationAggregationService.ConfirmForgotPasswordAsync(
            tokenId: tokenId,
            userId: userId,
            newPassword: newPassword,
            confirmNewPassword: confirmNewPassword);
}