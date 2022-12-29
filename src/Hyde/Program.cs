using Hyde.Builder;

var optionsFile = args.First();
if (!Path.IsPathRooted(optionsFile))
{
    optionsFile = Path.Join(Environment.CurrentDirectory, optionsFile);
}

Environment.CurrentDirectory = Path.GetDirectoryName(optionsFile) ?? Environment.CurrentDirectory;

var config = Configuration.CreateConfiguration(optionsFile);
var provider = Configuration.CreateServiceProvider(config);

var builder = provider.GetRequiredService<ISiteBuilder>();
var result = await builder.Build();

var factory = provider.GetRequiredService<ILoggerFactory>();
var logger = factory.CreateLogger("Hyde");

result.Log(logger, 20);
Thread.Sleep(1);
