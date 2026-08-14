using System.CommandLine;
using ApiForge.Cli.Commands;
using ApiForge.Generator.FileSystem;
using ApiForge.Generator.Generation;
using ApiForge.Generator.Rendering;
using ApiForge.Generator.Templates;

// Templates nằm ở gốc repo, tính đường dẫn tương đối từ thư mục chạy CLI
var templatesRoot = Path.Combine(AppContext.BaseDirectory, "../../../../../templates");
templatesRoot = Path.GetFullPath(templatesRoot);

var outputRoot = Directory.GetCurrentDirectory();

var fileSystem = new FileSystem();
var renderer = new TemplateRenderer();
var resolver = new TemplateResolver(templatesRoot);
var pipeline = new GenerationPipeline(fileSystem, renderer);
var generator = new ProjectGenerator(resolver, pipeline);

var rootCommand = new RootCommand("ApiForge — .NET API Starter Kit & Project Generator");
rootCommand.AddCommand(NewCommand.Create(generator, outputRoot));

return await rootCommand.InvokeAsync(args);
