using ApiForge.Core.Architecture;
using ApiForge.Core.Database;
using ApiForge.Core.Project;
using ApiForge.Core.Testing;
using Spectre.Console;

namespace ApiForge.Cli.Wizard;

public static class InteractiveWizard
{
    private const string Accent = "dodgerblue2";
    private const string Success = "springgreen2";

    public static ProjectDefinition? Run(string? initialName)
    {
        RenderBanner();

        var name = initialName;
        if (string.IsNullOrWhiteSpace(name))
        {
            name = AnsiConsole.Ask<string>("[bold]Project name:[/]");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(Rule("Architecture", Accent));
        var architecture = AnsiConsole.Prompt(
            new SelectionPrompt<ArchitectureStyle>()
                .AddChoices(ArchitectureStyle.VerticalSlice, ArchitectureStyle.CleanArchitecture, ArchitectureStyle.ModularMonolith));

        var useDdd = AnsiConsole.Confirm("Use DDD building blocks?", defaultValue: false);
        var useCqrs = AnsiConsole.Confirm("Use CQRS abstractions?", defaultValue: false);
        var useDomainEvents = AnsiConsole.Confirm("Use domain events?", defaultValue: false);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(Rule("Infrastructure", Accent));
        var database = AnsiConsole.Prompt(
            new SelectionPrompt<DatabaseProvider>()
                .AddChoices(DatabaseProvider.PostgreSQL, DatabaseProvider.None));

        var testFramework = AnsiConsole.Prompt(
            new SelectionPrompt<TestFramework>()
                .AddChoices(TestFramework.XUnit));

        var useDocker = AnsiConsole.Confirm("Include Docker setup?", defaultValue: true);

        var definition = new ProjectDefinition
        {
            Name = name!,
            Architecture = new ArchitectureOptions
            {
                Style = architecture,
                UseDdd = useDdd,
                UseCqrs = useCqrs,
                UseDomainEvents = useDomainEvents
            },
            Database = new DatabaseOptions { Provider = database },
            TestFramework = testFramework,
            UseDocker = useDocker
        };

        RenderSummary(definition);

        return definition;
    }

    private static void RenderBanner()
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(
            new FigletText("ApiForge")
                .Centered()
                .Color(Color.DodgerBlue2));

        AnsiConsole.WriteLine();

        var panel = new Panel(
            new Markup(
                "[grey]Welcome to[/] [bold dodgerblue2]ApiForge[/] [grey]— your .NET API starter-kit generator.[/]\n" +
                "[grey]Pick only what you need and we scaffold a buildable solution around it.[/]"))
            .BorderColor(Color.Grey42)
            .Padding(1, 2);

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    private static void RenderSummary(ProjectDefinition definition)
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(
            new FigletText("ApiForge")
                .Centered()
                .Color(Color.DodgerBlue2));

        AnsiConsole.WriteLine();

        var table = new Table()
            .BorderColor(Color.Grey42)
            .Title("[bold]Configuration Summary[/]")
            .AddColumn("Setting")
            .AddColumn("Value");

        table.AddRow("Project", $"[bold]{definition.Name.EscapeMarkup()}[/]");
        table.AddRow("Architecture", definition.Architecture.Style.ToString());
        table.AddRow("DDD", YesNo(definition.Architecture.UseDdd));
        table.AddRow("CQRS", YesNo(definition.Architecture.UseCqrs));
        table.AddRow("Domain events", YesNo(definition.Architecture.UseDomainEvents));
        table.AddRow("Database", definition.Database.Provider.ToString());
        table.AddRow("Test framework", definition.TestFramework.ToString());
        table.AddRow("Docker", YesNo(definition.UseDocker));

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private static string YesNo(bool value) =>
        value
            ? $"[{Success}]Yes[/]"
            : "[grey]No[/]";

    private static Rule Rule(string title, string color) =>
        new Rule($"[bold {color}]{title}[/]")
        {
            Justification = Justify.Left
        };
}
