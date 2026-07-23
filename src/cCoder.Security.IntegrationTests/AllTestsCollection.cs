// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Xunit;

namespace cCoder.Security.IntegrationTests;

[CollectionDefinition(nameof(AllTestsCollection))]
public sealed class AllTestsCollection : ICollectionFixture<AllTestsCollection>
{
}