using System.Reflection;
using Hyde.Builder;
using Hyde.Logging;
using Hyde.Mutator;
using Hyde.Mutator.Assets;
using Hyde.Mutator.Draft;
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
using Hyde.Services.ProjectResolver;
using Microsoft.Extensions.Logging.Console;

namespace Hyde;

internal static class Configuration
{
    public static IConfiguration LoadCoreConfiguration() => new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    public static IConfiguration ExtendProjectConfiguration(IConfiguration coreConfiguration, string projectFile) => new ConfigurationBuilder()
        .AddConfiguration(coreConfiguration, true)
        .AddJsonFile(projectFile, false)
        .Build();

    public static IServiceCollection LoadCoreServices(IConfiguration coreConfig) => new ServiceCollection()
        .AddLogger(coreConfig)
        .AddSingleton<IProjectResolver, ProjectResolver>();

    public static IServiceProvider LoadProjectServices(IServiceCollection coreServices, IConfiguration projectConfig) => new ServiceCollection()
        .AddRange(coreServices)
        .AddReader(projectConfig)
        .AddMutators(projectConfig)
        .AddSerializer(projectConfig)
        .AddBuilder()
        .BuildServiceProvider();

    private static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration) => services
        .AddLogging(builder => builder
            .AddConfiguration(configuration.GetSection("Logging"))
            .AddConsole(options => options.FormatterName = nameof(ColoredConsoleFormatter))
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
                    p.GetRequiredService<DraftMutator>(),
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
