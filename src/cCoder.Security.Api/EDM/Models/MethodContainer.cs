namespace cCoder.Security.Api.EDM.Models;

public class MethodContainer
{
    public string Name { get; set; }
    public ParameterContainer[] Parameters { get; set; }
    public ParameterContainer Returns { get; set; }
}