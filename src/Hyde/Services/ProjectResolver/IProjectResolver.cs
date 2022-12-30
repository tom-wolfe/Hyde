namespace Hyde.Services.ProjectResolver;

internal interface IProjectResolver
{
    FileInfo Resolve(FileInfo? projectFile);
}
