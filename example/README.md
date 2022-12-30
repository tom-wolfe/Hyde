# Hyde .NET Example

## Installation

To build the site, you must have the .NET 7.0 CLI installed. Then you can restore the dependencies using:

```dotnetcli
dotnet tool restore
```

## Usage

From the example folder, run the following command:

```powershell
dotnet tool run hyde "example.hyde.json"
```

The build site will be in the `dist` directory.
