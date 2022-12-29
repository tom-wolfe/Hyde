namespace Hyde.Mutator;

internal class AggregateMutator : ISiteMutator
{
    private readonly ISiteMutator[] _mutators;

    public AggregateMutator(IEnumerable<ISiteMutator> mutators)
    {
        this._mutators = mutators.ToArray();
    }

    public async Task<ISiteMutateResult> Mutate(Site site, CancellationToken cancellationToken = default)
    {
        var results = new List<ISiteMutateResult>();
        var duration = TimeSpan.Zero;
        foreach (var mutator in this._mutators)
        {
            var result = await mutator.Mutate(site, cancellationToken);
            results.Add(result);
        }

        return new AggregateSiteMutateResult
        {
            Name = "Mutate",
            Results = results
        };
    }
}
