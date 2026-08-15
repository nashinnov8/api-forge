# ApiForge

> **Status:** V1 — interactive wizard, 3 architecture templates (vertical-slice, clean-architecture, modular-monolith), DDD/CQRS fragments, auto SDK detection, and NuGet tool distribution all work end-to-end.

ApiForge is an opinionated .NET API starter-kit generator. Instead of a one-size-fits-all boilerplate, you pick only the infrastructure your project actually needs — database, cache, messaging, authentication — and ApiForge scaffolds a production-ready, buildable solution around it.

```
apiforge new
      │
      ▼
Interactive Wizard (Spectre.Console)
      │
      ▼
Project Definition
      │
      ▼
SDK Detection (auto-detect .NET version)
      │
      ▼
Template Resolver (by architecture style)
      │
      ▼
Fragment Resolver (conditional files)
      │
      ▼
Template Renderer (token replacement)
      │
      ▼
Generated .NET API
```

## Install

```bash
dotnet tool install --global ApiForge.Cli
```

Requires .NET SDK 8.0 or later on your machine.

## Usage

```bash
apiforge new <ProjectName>
# or just: apiforge new
```

The interactive wizard asks:

- Project name (optional — can pass as argument)
- Architecture style: `vertical-slice`, `clean-architecture`, `modular-monolith`
- DDD building blocks (Entity, AggregateRoot, ValueObject, IDomainEvent)
- CQRS abstractions (ICommand, ICommandHandler)
- Domain events
- Database provider: `PostgreSQL` or `None`
- Test framework: `xUnit`
- Docker setup

Then it:
1. Detects the highest installed .NET SDK/runtime on your machine.
2. Generates the project using the matching `TargetFramework` (net8.0, net9.0, net10.0...).
3. Creates a solution file and adds every generated `.csproj` to it.
4. Runs `dotnet restore` and `dotnet build` to verify the output compiles.

### Example

```bash
apiforge new MyApi
cd MyApi
dotnet run --project src/MyApi.Api
```

Expected output:

```
Generating MyApi...
✓ Generated 18 files at /path/to/MyApi
Running dotnet new sln -n MyApi...
✓ dotnet new sln -n MyApi succeeded
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

## Development

### Prerequisites

- .NET SDK 8.0 or later (see `global.json` — uses `rollForward: latestMajor`)

### Build

```bash
dotnet build ApiForge.slnx
```

### Run from source

```bash
dotnet run --project src/ApiForge.Cli -- new MyApi
```

### Tests

```bash
dotnet test ApiForge.slnx
```

Covers:
- `ApiForge.Core.Tests` — default values of `ProjectDefinition`, `TargetFramework`, `DotnetVersion`.
- `ApiForge.Generator.Tests` — template resolution per architecture, fragment activation (DDD, Docker), token replacement, generated file count.
- `ApiForge.Cli.Tests` — CLI command wiring.

## Project structure

```
src/
  ApiForge.Cli/         CLI entry point, wizard, SDK detection
    Wizard/             Spectre.Console interactive prompts
    Sdk/                Auto-detects .NET SDK/runtime version
    Infrastructure/     DotnetCli wrapper for running dotnet commands
  ApiForge.Core/        Domain models and options (framework-agnostic)
  ApiForge.Generator/   Template resolution, fragment resolution, rendering
templates/
  api/
    vertical-slice/     Vertical Slice architecture template
    clean-architecture/ Clean Architecture template
    modular-monolith/   Modular Monolith template (module-based structure)
  Each template has _fragments/ for conditional files:
    ddd/                Included when UseDdd = true
    cqrs/               Included when UseCqrs = true
    postgres/           Included when Database = PostgreSQL
    docker/             Included when UseDocker = true
tests/
  ApiForge.Core.Tests/
  ApiForge.Generator.Tests/
  ApiForge.Cli.Tests/
```

Dependency direction is enforced one-way:

```
ApiForge.Cli → ApiForge.Generator → ApiForge.Core
```

## How generation works

1. `InteractiveWizard` collects project options (name, architecture, DDD, CQRS, database, Docker).
2. `SdkDetector` detects the highest installed .NET SDK and sets `TargetFramework` (e.g. `net8.0`) and `DotnetVersion` (e.g. `8.0`).
3. `TemplateResolver` selects the template directory based on `ArchitectureStyle`.
4. `GenerationPipeline` copies base template files, replacing `{{ProjectName}}`, `{{TargetFramework}}`, and `{{DotnetVersion}}` tokens.
5. `FragmentResolver` includes conditional files from `_fragments/` based on the project definition.
6. `ApiForge.Cli` creates a solution, adds generated projects, then runs `restore` + `build`.

## Roadmap

- [x] Interactive CLI wizard (Spectre.Console)
- [x] Multi-template support (vertical-slice, clean-architecture, modular-monolith)
- [x] Feature fragments (DDD, CQRS, PostgreSQL, Docker)
- [x] Auto SDK detection (net8.0+)
- [x] NuGet tool distribution
- [x] `.apiforge/project.json` — persist configuration into generated project
- [ ] Non-interactive mode with CLI flags (`--database`, `--auth`, ...) for CI/CD use
- [ ] Redis cache, JWT authentication, ProblemDetails error handling
- [ ] Messaging (Kafka / RabbitMQ) + Outbox pattern
- [ ] OpenTelemetry + Serilog observability
- [ ] Feature/entity generators (`apiforge feature add`, `apiforge entity add`)
- [ ] Template management (`apiforge template list/install`)

## Contributing

This is currently a solo learning/portfolio project and not yet accepting external contributions, but issues and suggestions are welcome.

## License

See [LICENSE](LICENSE).
