namespace Hyde.Mutator;

internal interface ISiteMutator
{
    Task<ISiteMutateResult> Mutate(Site site, CancellationToken cancellationToken = default);
}
