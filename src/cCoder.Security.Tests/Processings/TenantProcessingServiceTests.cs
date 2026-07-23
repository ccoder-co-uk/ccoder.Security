// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Tests.Processings;

public partial class TenantProcessingServiceTests
{
    private readonly Mock<ITenantService> tenantServiceMock;
    private readonly ITenantProcessingService tenantProcessingService;

    public TenantProcessingServiceTests()
    {
        tenantServiceMock = new Mock<ITenantService>();
        tenantProcessingService = new TenantProcessingService(tenantServiceMock.Object);
    }

    public Tenant[] RandomTenants() =>
        Enumerable.Range(1, new Random().Next(1, 20))
            .Select(selector: _ => RandomTenant())
            .ToArray();

    public Tenant RandomTenant() =>
        GetTenantFiller().Create();

    public Filler<Tenant> GetTenantFiller()
    {
        Filler<Tenant> filler = new();

        filler.Setup()
            .OnType<DateTimeOffset>().Use(DateTimeOffset.Now)
            .OnProperty(p => p.Analysis).IgnoreIt()
            .OnProperty(p => p.UserEvents).IgnoreIt()
            .OnProperty(property: p => p.Roles).IgnoreIt();

        return filler;
    }
}