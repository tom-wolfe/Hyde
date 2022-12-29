using Microsoft.Extensions.Logging.Console;
using System.Reflection;
using Hyde.Builder;
using Hyde.Logging;
using Hyde.Mutator;
using Hyde.Mutator.Assets;
using Hyde.Mutator.Link;
using Hyde.Mutator.Markdown;
using Hyde.Mutator.Metadata;
using Hyde.Mutator.Search;
using Hyde.Mutator.Styles;
using Hyde.Mutator.Tags;
using Hyde.Mutator.Tasks;
using Hyde.Mutator.Template;
using Hyde.Mutator.Template.Functions;
using Hyde.Mutator.Template.Layout;
using Hyde.Reader;
using Hyde.Serializer;
using Hyde.Services.FileFinder;
using Hyde.Services.LinkResolver;
using Hyde.Services.Metadata;

namespace Hyde;

internal static class Configuration
{
    public static IConfiguration CreateConfiguration(string projectFile)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile(projectFile)
            .Build();
        return configuration;
    }

    public static IServiceProvider CreateServiceProvider(IConfiguration configuration)
    {
        var provider = new ServiceCollection()
            .AddSingleton(configuration)
            .AddLogger(configuration)
            .AddReader(configuration)
            .AddMutators(configuration)
            .AddSerializer(configuration)
            .AddBuilder()
            .BuildServiceProvider();
        return provider;
    }

    private static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration) => services
        .AddLogging(builder => builder
            .AddConfiguration(configuration.GetSection("Logging"))
            .AddConsole(options => options.FormatterName = "testFormatter")
            .AddConsoleFormatter<ColoredConsoleFormatter, ConsoleFormatterOptions>()
        );

    private static IServiceCollection AddReader(this IServiceCollection services, IConfiguration configuration) => services
        .Configure<SiteReaderOptions>(configuration.GetSection("read"))
        .AddSingleton<ISiteReader, SiteReader>();

    private static IServiceCollection AddMutators(this IServiceCollection services, IConfiguration configuration)
    {
        // Add known types
        services
            .AddSingleton<IMetadataExtractor, YamlMetadataExtractor>()
            .AddSingleton<ILayoutStore, FileSystemLayoutStore>()
            .AddSingleton<IFileFinder, FileFinder>()
            .Configure<LinkResolverOptions>(configuration.GetSection("mutate:linkMutator"))
            .AddSingleton<ITemplateContextFactory, TemplateContextFactory>()
            .AddSingleton<ICustomTemplateFunctions, CustomTemplateFunctions>()
            .AddSingleton<ILinkResolver, LinkResolver>()
            .AddSingleton<ISiteMutator>(p =>
            {
                var mutators = new List<ISiteMutator>
                {
                    p.GetRequiredService<MetadataMutator>(),
                    p.GetRequiredService<TasksMutator>(),
                    p.GetRequiredService<TagsMutator>(),
                    p.GetRequiredService<MarkdownMutator>(),
                    p.GetRequiredService<SearchMutator>(),
                    p.GetRequiredService<TemplateMutator>(),
                    p.GetRequiredService<StylesMutator>(),
                    p.GetRequiredService<AssetsMutator>(),
                    p.GetRequiredService<LinkMutator>()
                };
                return new AggregateMutator(mutators);
            });

        // Add the dynamic mutator types.
        var mutatorTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(ISiteMutator)) && !t.IsAbstract);

        foreach (var mutatorType in mutatorTypes)
        {
            var constructor = mutatorType.GetConstructors().First();
            var optionsParameter = constructor.GetParameters().FirstOrDefault(p => p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(IOptions<>));
            if (optionsParameter != null)
            {
                var configSection = configuration.GetSection("Mutate:" + mutatorType.Name);
                var optionsType = optionsParameter.ParameterType.GetGenericArguments().First();
                services.ConfigureOptionsByType(optionsType, configSection);
            }

            services.TryAddSingleton(mutatorType);
        }

        return services;
    }

    private static IServiceCollection AddSerializer(this IServiceCollection services, IConfiguration configuration) => services
        .Configure<SiteSerializerOptions>(configuration.GetSection("serialize"))
        .AddSingleton<ISiteSerializer, SiteSerializer>();

    private static IServiceCollection AddBuilder(this IServiceCollection services) => services
        .AddSingleton<ISiteBuilder, SiteBuilder>();
}