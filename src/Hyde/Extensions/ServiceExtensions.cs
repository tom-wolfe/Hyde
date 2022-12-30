using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hyde.Extensions;

internal static class ServiceExtensions
{
    public static IServiceCollection ConfigureOptionsByType(this IServiceCollection services, Type optionsType, IConfiguration configuration)
    {
        var configureMethod = typeof(OptionsServiceCollectionExtensions)
            .GetMethods()
            .Select(m => new
            {
                Method = m,
                Parameters = m.GetParameters(),
                Args = m.GetGenericArguments()
            })
            .Where(m => m.Method.Name == "Configure"
                   && m.Method.IsStatic
                   && m.Parameters.Length == 2)
            .Select(m => m.Method)
            .First();

        var typedConfigureMethod = configureMethod.MakeGenericMethod(optionsType);

        var parameters = new object?[]
        {
            services,
            (Action<object>)configuration.Bind
        };

        typedConfigureMethod.Invoke(null, BindingFlags.InvokeMethod, null, parameters, null);

        return services;
    }

    public static IServiceCollection AddRange(this IServiceCollection services, IServiceCollection source)
    {
        foreach (var service in source)
        {
            services.Add(service);
        }
        return services;
    }
}
