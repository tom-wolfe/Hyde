using Hyde.Mutator.Link;
using Hyde.Services.FileFinder;
using Hyde.Services.LinkResolver;

namespace Hyde.Tests.Unit.Mutator;

public class LinkMutatorTests
{
    private readonly LinkMutator _mutator;

    private readonly Mock<ILogger<LinkMutator>> _mockLogger = new();
    private readonly Mock<IFileFinder> _mockFinder = new();
    private readonly Mock<ILinkResolver> _mockResolver = new();

    public LinkMutatorTests()
    {
        this._mutator = new LinkMutator(this._mockLogger.Object, this._mockFinder.Object, this._mockResolver.Object);
    }

    [Fact]
    public async Task Face()
    {
        // Arrange
        var site = new Site("");
        var root = new SiteDirectory("");
        var file = new VirtualSiteFile(
            "file.html",
            "<html><body><a href=\"edyria\" title=\"Neferi Khamet\"></a></body></html>"
        );
        root.AddFile(file);
        site.SetRoot(root);

        // Act
        await this._mutator.Mutate(site);

        // Assert

    }
}
