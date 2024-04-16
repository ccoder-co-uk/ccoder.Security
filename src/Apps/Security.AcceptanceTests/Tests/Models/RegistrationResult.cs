using cCoder.Security.Objects.Entities;

namespace cCoder.Security.AcceptanceTests.Tests.Models;

public class RegistrationResult
{
    public SSOUser User { get; set; }
    public string Token { get; set; }
}
