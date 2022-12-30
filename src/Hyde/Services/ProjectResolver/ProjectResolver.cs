namespace Hyde.Services.ProjectResolver;

internal class ProjectResolver : IProjectResolver
{
    private const string DefaultProject = "hyde.json";
    private const string DefaultSearch = "*." + DefaultProject;

    private readonly ILogger<ProjectResolver> _logger;

    public ProjectResolver(ILogger<ProjectResolver> logger)
    {
        this._logger = logger;
    }

    public FileInfo Resolve(FileInfo? projectFile)
    {
        // If we haven't given a file, default to current directory.
        if (projectFile == null)
        {
            this._logger.LogDebug("No project file specified. Scanning current directory.");
            projectFile = new FileInfo(Environment.CurrentDirectory);
        }

        // If we have a directory, try and locate a project file.
        var attributes = File.GetAttributes(projectFile.FullName);
        if (attributes.HasFlag(FileAttributes.Directory))
        {
            this._logger.LogDebug("Scanning directory: {Directory}", projectFile.FullName);
            projectFile = FindProjectFileInDirectory(projectFile.FullName);
        }

        if (!projectFile.Exists)
        {
            throw new ApplicationException("Unable to find Hyde project file.");
        }

        this._logger.LogInformation("Resolved project: {Path}", projectFile.FullName);
        return projectFile;
    }

    private FileInfo FindProjectFileInDirectory(string projectDirectory)
    {
        // Check for a default project file.
        var defaultPath = Path.Join(projectDirectory, DefaultProject);
        if (File.Exists(defaultPath))
        {
            this._logger.LogDebug("Found project file with default name.");
            return new FileInfo(defaultPath);
        }

        var firstProject = Directory.GetFiles(projectDirectory, DefaultSearch).FirstOrDefault();
        if (firstProject != null)
        {
            this._logger.LogDebug("Found one or more project files with correct extension.");
            return new FileInfo(firstProject);
        }

        throw new ApplicationException("Unable to find Hyde project file in directory.");
    }
}
