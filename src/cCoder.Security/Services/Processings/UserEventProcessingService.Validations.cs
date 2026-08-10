// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Processings;

internal sealed partial class UserEventProcessingService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }

    private static void ValidateUserEventOnAdd(UserEvent newUserEvent) =>
        Validate(inputs: newUserEvent);

    private static void ValidateUserEventOnDelete(UserEvent deletedUserEvent) =>
        Validate(inputs: deletedUserEvent);

}