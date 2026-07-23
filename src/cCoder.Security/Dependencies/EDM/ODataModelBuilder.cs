// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using Microsoft.OData.ModelBuilder;
using System.Linq.Expressions;

namespace cCoder.Security.Dependencies.EDM;

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
        EntitySetConfiguration<T> setConfig = Builder.EntitySet<T>(name: setName);

        StructuralTypeConfiguration typeInfo = Builder.StructuralTypes.First(predicate: t => t.ClrType == typeof(T));

        return setConfig;
    }

    protected virtual EntitySetConfiguration<T> AddJoinSet<T, TKey>(Expression<Func<T, TKey>> key)
        where T : class
    {
        string setName = typeof(T).Name;
        EntitySetConfiguration<T> setConfig = Builder.EntitySet<T>(name: setName);

        _ = Builder
            .EntityType<T>()
            .HasKey(keyDefinitionExpression: key);

        return setConfig;
    }
}