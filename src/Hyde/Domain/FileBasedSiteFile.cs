namespace Hyde.Domain;

[DebuggerDisplay("{SourcePath,nq}")]
internal class FileBasedSiteFile : SiteFile
{
    public FileBasedSiteFile(string sourcePath) : base(Path.GetFileName(sourcePath))
    {
        if (!Path.IsPathFullyQualified(sourcePath))
        {
            sourcePath = Path.Join(Environment.CurrentDirectory, sourcePath);
        }
        SourcePath = sourcePath;
    }

    public string SourcePath { get; }

    protected override Stream GetOriginalContentStream() => File.OpenRead(SourcePath);
}
