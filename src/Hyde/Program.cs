using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using Hyde.Builder;
using Hyde.Services.ProjectResolver;

namespace Hyde;

public static class Program
{
    public static Task<int> Main(string[] args)
    {
        var projectArgument = new Argument<FileInfo?>
        {
            Name = "project",
            Description = "The path to the project JSON file",
            Arity = ArgumentArity.ZeroOrOne
        };

        var command = new RootCommand("Extendable static site generation");
        command.AddArgument(projectArgument);
        command.SetHandler(BuildSite, projectArgument);

        var parser = new CommandLineBuilder(command)
            .UseHelp()
            .UseVersionOption()
            .UseExceptionHandler(HandleException)
            .CancelOnProcessTermination()
            .Build();

        return parser.InvokeAsync(args);
    }

    private static async Task BuildSite(FileInfo? projectFile)
    {
        // Load application-wide config.
        var coreConfig = Configuration.LoadCoreConfiguration();
        var coreServices = Configuration.LoadCoreServices(coreConfig);
        await using var coreProvider = coreServices.BuildServiceProvider();

        // Find an appropriate JSON project file.
        var projectResolver = coreProvider.GetRequiredService<IProjectResolver>();
        projectFile = projectResolver.Resolve(projectFile);
        Environment.CurrentDirectory = Path.GetDirectoryName(projectFile.FullName)!;

        // Merge the core and project config, and add project-specific services.
        var projectConfig = Configuration.ExtendProjectConfiguration(coreConfig, projectFile.FullName);
        var projectProvider = Configuration.LoadProjectServices(coreServices, projectConfig);

        // Build the site.
        var builder = projectProvider.GetRequiredService<ISiteBuilder>();
        var result = await builder.Build();

        // Log the result to the console.
        var factory = projectProvider.GetRequiredService<ILoggerFactory>();
        var logger = factory.CreateLogger("");
        result.Write(logger, 20);
    }

    private static void HandleException(Exception exception, InvocationContext context)
    {
        var writer = context.Console.Error.CreateTextWriter();
        writer.WriteLineColored("An unhandled exception has occurred: ", ConsoleColors.Error);
        writer.WriteLineColored(exception.ToString(), ConsoleColors.Error);
        context.ExitCode = 1;
    }
}
