// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using Microsoft.OData.ModelBuilder;
using System.Linq.Expressions;

namespace cCoder.Security.Exposures.EDM;

/// <summary>
/// Base model builder class for all OData model builders
/// </summary>
public abstract class ODataModelBuilder
{
    protected ODataConventionModelBuilder Builder = new();

    /// <summary>
    /// Derived types implement this to setup the OData Model information
    /// </summary>
    /// <returns></returns>
    public abstract ODataModel Build();

    protected virtual EntitySetConfiguration<T> AddSet<T, TKey>(bool enableBatchingToo = false, string setName = null)
        where T : class
    {
        setName ??= typeof(T).Name;
        return Builder.EntitySet<T>(name: setName);
    }

    protected virtual EntitySetConfiguration<T> AddJoinSet<T, TKey>(Expression<Func<T, TKey>> key)
        where T : class =>
        (
            Set: Builder.EntitySet<T>(name: typeof(T).Name),
            Key: Builder.EntityType<T>()
                .HasKey(keyDefinitionExpression: key)
        )
            .Set;
}
