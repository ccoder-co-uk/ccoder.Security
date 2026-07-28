// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Xunit;

namespace Security.AcceptanceTests;

[CollectionDefinition(nameof(AllTestsCollection))]
public class AllTestsCollection :
    ICollectionFixture<SecurityAcceptanceTestFixture>
{
}