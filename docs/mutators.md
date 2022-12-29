# Mutators

## Core Mutators

### MetadataMutator

Typically the first mutator that runs is the `MetadataMutator` its purpose is to strip YAML frontmatter from text files and load it into a dictionary on the site file object that can be read by other mutators.

#### Options

```json
{
  "default": { 
    // A set of key value pairs describing the default metadata for each page.
  }
}
```

### MarkdownMutator

This mutator uses [Markdig](https://github.com/xoofx/markdig) to convert the contents of any `.md` files in the site to HTML fragments.

### TemplateMutator

This mutator uses [Scriban](https://github.com/scriban/scriban) to render any templating used in the site. Each page is loaded with a context object that makes several useful objects available for rendering:

* `site` this is the current `Site` domain object.
* `page` this is current `SiteFile` domain object.
* `directory` this is the parent `SiteDirectory` domain object of the current `SiteFile`.

#### Scriban Functions

In addition to the above context objects, some [custom functions](https://github.com/scriban/scriban/blob/master/doc/language.md#89-function-call-expression) are made available.

* `link`. Takes in a `SiteFile` instance and creates a URL for it.
* `breadcrumbs`. Takes in a `SiteFile` instance and returns the Index file for each parent directory up to the site root.
* `sluggify`. Takes a string and returns the kebab case version of it.

#### Options

```json
{
  "defaultTemplate": "The name of a template to use when no other is specified.",
  "includeDirectories": [
    // An array of directories relative to the project root to include when searching for template files.
  ]
}
```

### StylesMutator

This mutator will take a link to the entry stylesheet provided in the site project file, convert it from SASS to CSS using [LibSassHost](https://github.com/Taritsyn/LibSassHost)

#### Options

```json
{
  "stylesheet": "The path to the CSS file that will be created, relative to the compiled site's root.",
  "includeDirectories": [
    "./common/styles"
    // An array of directories relative to the project root to search for SASS includes.
  ]
}
```

### AssetsMutator

Copies static files from the source to the target without putting through the rest of the pipeline.

#### Options

```json
{
  "assets": {
    "./common/scripts": "/scripts"
    // A set of key value pairs where the key is a source file or directory that will be copied to the location specified in the value, relative to the compiled site's root.
  }
}
```

### LinkMutator

Handles link resolution so that links to local files are maintained after the site is built.

**TODO:** This also allows for lookup links and shorthand links. Describe both of those.
