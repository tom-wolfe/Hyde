# Hyde

An extendable static site generator written in .NET

## Phases

Hyde has three distinct operating phases:

1. **Read.** The site file structure is read and loaded into the domain model.
2. **Mutate.** The domain model is passed through a pipeline of mutator objects that handle things like markdown->HTML conversion, link resolution, etc.
3. **Serialize.** The final domain model is serialized to disk.

## Pipelines and Mutators

The site domain model is just a series of directory and file objects with some standard functionality like metadata and the ability to remove/rename/change file extensions. Once the site domain model has been loaded, it is then passed through the mutation pipeline to produce the final site that is then written to disk.

The mutation pipeline is made up of a series of Independent but composable pieces of functionality called 'mutators' that are run in serial to build up the site in layers. Mutators typically recurse through each file in the site, look at the contents, make any appropriate changes, then move on, but they can also make changes to the structure of the site, adding or removing files and directories. 

See [Mutators](./docs/mutators.md) for more info.

## Extension

**TODO:** Describe the `ISiteMutator` interface and the base classes: `FileMutator`, `CollectorMutator` and `AggregateMutator`.

## Options

**TODO:** Describe how the IOptions pattern is used to load mutator settings.
