# ApiForge

> **Status:** Early stage (V1) — vertical-slice + PostgreSQL scaffolding works end-to-end via the CLI. Interactive wizard, DDD/CQRS toggles, messaging, and config persistence are planned (see [Roadmap](#roadmap)).

ApiForge is an opinionated .NET API starter-kit generator. Instead of a one-size-fits-all boilerplate, you pick only the infrastructure your project actually needs — database, cache, messaging, authentication — and ApiForge scaffolds a production-ready, buildable solution around it.

```
apiforge new
      │
      ▼
Interactive Wizard *(planned)*
      │
      ▼
Project Definition
      │
      ▼
Template Resolver
      │
      ▼
Template Renderer
      │
      ▼
Generated .NET API
```

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

The `new` command:
1. Generates a project into the current directory from the default template.
2. Creates a solution file (`.slnx`) and adds every generated `.csproj` to it.
3. Runs `dotnet restore` and `dotnet build` on the generated solution.

### Example

```bash
dotnet run --project src/ApiForge.Cli -- new MyApi
cd MyApi
dotnet run --project src/MyApi.Api
```

Expected output:

```
Generating MyApi...
✓ Generated 10 files at /path/to/MyApi
Running dotnet new sln -n MyApi...
✓ dotnet new sln -n MyApi succeeded
Running dotnet sln add src/MyApi.Api/MyApi.Api.csproj...
✓ dotnet sln add src/MyApi.Api/MyApi.Api.csproj succeeded
...
Running dotnet restore...
✓ dotnet restore succeeded
Running dotnet build...
✓ dotnet build succeeded

Next steps:
  cd /path/to/MyApi
  dotnet run --project src/MyApi.Api
```

Then verify it's alive:

```bash
curl http://localhost:5000/health
# {"status":"ok","project":"MyApi"}
```

## Tests

```bash
dotnet test ApiForge.slnx
```

Covers:
- `ApiForge.Core.Tests` — default values and shape of `ProjectDefinition`.
- `ApiForge.Generator.Tests` — end-to-end generation pipeline (token replacement in file names/content, template manifest exclusion, generated file count).

## Project structure

```
src/
  ApiForge.Cli/        CLI entry point and commands
  ApiForge.Core/       Domain models and options (framework-agnostic, no dependencies)
  ApiForge.Generator/  Template resolution, rendering, and generation pipeline
templates/
  api/default/         Default API template (Vertical Slice, PostgreSQL, xUnit, Docker)
tests/
  ApiForge.Core.Tests/
  ApiForge.Generator.Tests/
  ApiForge.Cli.Tests/
samples/                Example generated project(s)
docs/
  architecture/         Design decisions and diagrams
  guides/
  decisions/
```

Dependency direction is enforced one-way:

```
ApiForge.Cli → ApiForge.Generator → ApiForge.Core
```

`ApiForge.Core` has no project references. `ApiForge.Generator` never references `ApiForge.Cli`.

## How generation works

1. `ApiForge.Cli` resolves the template root relative to the CLI's output directory.
2. `TemplateResolver` selects a template based on the project definition (currently always `api/default`).
3. `GenerationPipeline` walks every file in the template, replacing `{{ProjectName}}` tokens in both file/folder names and file content.
4. `ApiForge.Cli` creates a solution file, adds every generated `.csproj` to it, then runs `restore` and `build` to confirm the output actually compiles.

## Roadmap

- [x] V1 — vertical-slice scaffolding, PostgreSQL, xUnit, automatic solution creation + restore/build
- [ ] Interactive CLI wizard (Spectre.Console) with dynamic, dependency-aware questions
- [ ] Non-interactive mode with CLI flags (`--database`, `--auth`, ...) for CI/CD use
- [ ] `.apiforge/project.json` — persist the chosen configuration into the generated project
- [ ] DDD building blocks (Entity, AggregateRoot, ValueObject, DomainEvent)
- [ ] CQRS abstraction (`ICommand`, `ICommandHandler`, `IQuery`)
- [ ] Redis cache, JWT authentication, ProblemDetails error handling
- [ ] Messaging (Kafka / RabbitMQ) + Outbox pattern
- [ ] OpenTelemetry + Serilog observability
- [ ] Feature/entity generators (`apiforge feature add`, `apiforge entity add`)
- [ ] Template management (`apiforge template list/install`)

## Contributing

This is currently a solo learning/portfolio project and not yet accepting external contributions, but issues and suggestions are welcome.

## License

See [LICENSE](LICENSE).
