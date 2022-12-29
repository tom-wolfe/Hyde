namespace Hyde.Mutator;

/// <summary>
/// Provides the contractor for mutating a given <see cref="Site"/> object in place.
/// </summary>
internal interface ISiteMutator
{
    /// <summary>
    /// Mutates the site object.
    /// </summary>
    /// <param name="site">The site to mutate.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>Information about the mutation operation.</returns>
    Task<ISiteMutateResult> Mutate(Site site, CancellationToken cancellationToken = default);
}
