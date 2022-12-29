namespace Hyde.Tests.Unit.Domain;

public class SiteDirectoryTests
{
    [Fact]
    public void UnnamedRoot_RelativePath_FullPath()
    {
        // Arrange
        var root = new SiteDirectory("");

        // Act
        var path = root.GetRelativePath();

        // Assert
        path.Should().Be("/");
    }

    [Fact]
    public void NamedRoot_RelativePath_FullPath()
    {
        // Arrange
        var root = new SiteDirectory("test");

        // Act
        var path = root.GetRelativePath();

        // Assert
        path.Should().Be("/test");
    }

    [Fact]
    public void NamedRootWithSubDirectory_RelativePath_FullPath()
    {
        // Arrange
        var root = new SiteDirectory("root");
        var sub = new SiteDirectory("sub");
        root.AddDirectory(sub);

        // Act
        var path = sub.GetRelativePath();

        // Assert
        path.Should().Be("/root/sub");
    }

    [Fact]
    public void NamedRootWithSubDirectoryAndFile_RelativePath_FullPath()
    {
        // Arrange
        var root = new SiteDirectory("root");
        var sub = new SiteDirectory("sub");
        root.AddDirectory(sub);

        var file = new VirtualSiteFile("file.md");
        sub.AddFile(file);

        // Act
        var path = file.GetRelativePath();

        // Assert
        path.Should().Be("/root/sub/file.md");
    }

    [Fact]
    public void UnnamedRootWithSubDirectoryAndFile_RelativePath_FullPath()
    {
        // Arrange
        var root = new SiteDirectory("");
        var sub = new SiteDirectory("sub");
        root.AddDirectory(sub);

        var file = new VirtualSiteFile("file.md");
        sub.AddFile(file);

        // Act
        var path = file.GetRelativePath();

        // Assert
        path.Should().Be("/sub/file.md");
    }

    [Fact]
    public void UnnamedRootWithIndexFile_RelativePath_FullPath()
    {
        // Arrange
        var root = new SiteDirectory("");

        var file = new VirtualSiteFile("index.md");
        root.AddFile(file);

        // Act
        var path = file.GetRelativePath();

        // Assert
        path.Should().Be("/");
    }
}
