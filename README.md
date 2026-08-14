# ApiForge

ApiForge is a .NET API starter-kit generator. It scaffolds a complete, production-ready API solution from a template, then restores and builds it for you.

## Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) (see `global.json` for the exact version)

## Build

```bash
dotnet build ApiForge.slnx
```

## Run the CLI

```bash
dotnet run --project src/ApiForge.Cli -- new <ProjectName>
```

The `new` command generates a project into the current directory, creates a solution, adds the generated projects to it, and runs `restore` + `build`.

Example:

```bash
dotnet run --project src/ApiForge.Cli -- new MyApi
cd MyApi
dotnet run --project src/MyApi.Api
```

## Tests

```bash
dotnet test ApiForge.slnx
```

## Project structure

```
src/
  ApiForge.Cli/        CLI entry point and commands
  ApiForge.Core/       Domain models and options
  ApiForge.Generator/  Template resolution, rendering, and generation pipeline
templates/
  api/default/         Default API template
tests/                 Unit test projects
```

## How generation works

1. `ApiForge.Cli` resolves the template root relative to the CLI output.
2. `TemplateResolver` selects a template based on the project definition.
3. `GenerationPipeline` copies the template, replacing `{{ProjectName}}` tokens in file names and content.
4. The CLI creates a solution, adds generated projects, and restores/builds them.
