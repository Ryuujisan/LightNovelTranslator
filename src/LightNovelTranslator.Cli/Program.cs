using LightNovelTranslator.Core;
using LightNovelTranslator.Docx;
using LightNovelTranslator.Ollama;

var command = args.Length > 0 ? args[0] : "translate";

var inputFile = GetArg(args, "--input") ?? "test.docx";
var outputFile = GetArg(args, "--output") ?? "test_translated.docx";
var progressFile = GetArg(args, "--progress") ?? "test_translated.progress.json";

var reader = new DocxDocumentReader();
var progressStore = new TranslationProgressStore();

var translator = new DocxTranslator(
    new OllamaTranslator(),
    progressStore);

var writer = new DocxDocumentWriter();

switch (command)
{
    case "translate":
    {
        var document = await reader.ReadAsync(inputFile);

        var translatedDocument =
            await translator.TranslateAsync(document);

        await writer.WriteAsync(
            inputFile,
            outputFile,
            translatedDocument);

        Console.WriteLine("Done.");
        break;
    }

    case "resume":
    {
        var progress =
            await progressStore.LoadAsync(progressFile);

        var document =
            await reader.ReadAsync(progress.InputPath);

        var translatedDocument =
            await translator.ResumeAsync(document, progress);

        await writer.WriteAsync(
            progress.InputPath,
            progress.OutputPath,
            translatedDocument);

        Console.WriteLine("Resume done.");
        break;
    }

    default:
        Console.WriteLine("Unknown command.");
        Console.WriteLine("Usage:");
        Console.WriteLine("  translate --input test.docx --output test_translated.docx");
        Console.WriteLine("  resume --progress test_translated.progress.json");
        break;
}

static string? GetArg(string[] args, string name)
{
    var index = Array.IndexOf(args, name);

    if (index < 0 || index + 1 >= args.Length)
        return null;

    return args[index + 1];
}