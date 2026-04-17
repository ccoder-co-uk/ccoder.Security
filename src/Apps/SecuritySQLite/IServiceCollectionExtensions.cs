namespace SecuritySQLite;

public static partial class IServiceCollectionExtensions
{
    public static void AddAspNetCore(this IServiceCollection services)
    {
        _ = services.AddResponseCompression();
        _ = services.AddMvcCore(options =>
        {
            options.MaxIAsyncEnumerableBufferLimit = int.MaxValue;
            options.MaxModelBindingCollectionSize = 10000;
            options.MaxModelBindingRecursionDepth = 10;
        })
            .AddDataAnnotations()
            .AddCors(options => options.AddDefaultPolicy(builder =>
            {
                _ = builder.AllowAnyHeader();
                _ = builder.AllowAnyMethod();
                _ = builder.SetIsOriginAllowed(origin => true);
                _ = builder.AllowCredentials();
            }));
    }

    public static void AddMetadata(this IServiceCollection services)
    {
        _ = services.AddEndpointsApiExplorer();
        _ = services.AddSwaggerGen();
    }
}
