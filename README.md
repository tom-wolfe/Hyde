# Hyde

![Nuget](https://img.shields.io/nuget/v/HydeDotNet) ![GitHub](https://img.shields.io/github/license/tom-wolfe/Hyde) ![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/tom-wolfe/Hyde/ci.yml) ![Libraries.io dependency status for latest release](https://img.shields.io/librariesio/release/nuget/HydeDotNet)

Hyde is an extendable static site generator written in .NET with support for templating, markdown, frontmatter and SASS via libraries like [Scriban](https://github.com/scriban/scriban), [Markdig](https://github.com/xoofx/markdig), [YamlDotNet](https://github.com/aaubry/YamlDotNet) and [LibSassHost](https://github.com/Taritsyn/LibSassHost).

## Installation

Hyde is installed as a [dotnet tool](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools) using the following command:

```dotnetcli
dotnet tool install HydeDotNet
```

Note that installing dotnet tools locally requires a [dotnet-tools manifest](https://learn.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use#create-a-manifest-file).
It can also be installed globally without requiring a manifest file:

```dotnetcli
dotnet tool install -g HydeDotNet
```

## Usage

Once Hyde has been installed, it can be invoked using the `hyde` command if installed globally, or `dotnet tool run hyde` from the project directory if installed locally. Hyde takes one argument: the path to the JSON project file with the site settings in it.

[See a working example here.](./example/README.md)

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
