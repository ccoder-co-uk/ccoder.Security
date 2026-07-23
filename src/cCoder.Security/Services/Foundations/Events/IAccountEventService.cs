// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Events;

internal interface IAccountEventService
{
    ValueTask RaiseRegistrationCreatedSSOUserRegisterUserEventAsync(
        SSOUser user,
        RegisterUser registerForm,
        string token);

    ValueTask RaiseRegistrationConfirmedSSOUserEventAsync(SSOUser user, string token);

    ValueTask RaiseInvitationCreatedSSOUserRegisterUserEventAsync(
        SSOUser user,
        RegisterUser registerForm,
        string token);

    ValueTask RaiseInvitationAcceptedSSOUserRegisterUserEventAsync(
        SSOUser user,
        RegisterUser registerForm,
        string token);

    ValueTask RaisePasswordResetRequestedSSOUserEventAsync(SSOUser user, string token);
}