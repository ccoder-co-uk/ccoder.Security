// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Events;

internal interface IAccountEventService
{
    ValueTask RaiseRegistrationCreatedEventAsync(SSOUser user, RegisterUser registerForm, string token);

    ValueTask RaiseRegistrationConfirmedEventAsync(SSOUser user, string token);

    ValueTask RaiseInvitationCreatedEventAsync(SSOUser user, RegisterUser registerForm, string token);

    ValueTask RaiseInvitationAcceptedEventAsync(SSOUser user, RegisterUser registerForm, string token);

    ValueTask RaisePasswordResetRequestedEventAsync(SSOUser user, string token);
}