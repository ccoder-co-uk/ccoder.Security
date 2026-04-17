using cCoder.Security.Brokers.DateTime;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations;
using cCoder.Security.Services.Foundations.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Tests.Foundations;

public partial class TenantServiceTests
{
    private readonly Mock<ITenantBroker> tenantBrokerMock;
    private readonly Mock<ISecurityDateTimeOffsetBroker> dateTimeOffsetBrokerMock;
    private readonly ITenantService tenantService;

    public TenantServiceTests()
    {
        tenantBrokerMock = new Mock<ITenantBroker>();
        dateTimeOffsetBrokerMock = new Mock<ISecurityDateTimeOffsetBroker>();
        tenantService = new TenantService(tenantBrokerMock.Object);
    }

    private Tenant[] RandomTenants() => 
        Enumerable.Range(1, new Random().Next(10, 20))
            .Select(_ => RandomTenant())
            .ToArray();

    private Tenant RandomTenant() => 
        GetTenantFiller().Create();

    private Filler<Tenant> GetTenantFiller()
    {
        Filler<Tenant> filler = new();

        filler.Setup()
            .OnType<DateTimeOffset>().Use(DateTimeOffset.Now)
            .OnProperty(t => t.Analysis).IgnoreIt()
            .OnProperty(t => t.UserEvents).IgnoreIt()
            .OnProperty(t => t.Roles).IgnoreIt();

        return filler;
    }
}


