// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Dependencies;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class UserEventService
{
    private static void Validate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateUserEventOnAdd(UserEvent newUserEvent) =>
        Validate(inputs: newUserEvent);

    private static void ValidateUserEventOnDelete(UserEvent deletedUserEvent) =>
        Validate(inputs: deletedUserEvent);
}