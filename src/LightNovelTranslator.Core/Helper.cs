namespace LightNovelTranslator.Core;

public static class Helper
{
    public static string ResolveOutputPath(
        string inputPath,
        string? outputPath,
        string language)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            var directory = Path.GetDirectoryName(inputPath)!;
            var fileName = Path.GetFileNameWithoutExtension(inputPath);

            return Path.Combine(
                directory,
                $"{fileName}_{language}.docx");
        }

        if (!Path.HasExtension(outputPath))
        {
            Directory.CreateDirectory(outputPath);

            var fileName = Path.GetFileNameWithoutExtension(inputPath);

            return Path.Combine(
                outputPath,
                $"{fileName}_{language}.docx");
        }

        return outputPath;
    }
    
    public static string[] GetDocumentsPaths(string inputPath, string extension)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            return [];

        extension = extension.TrimStart('.');

        if (File.Exists(inputPath))
        {
            return Path.GetExtension(inputPath).Equals($".{extension}", StringComparison.OrdinalIgnoreCase)
                ? [inputPath]
                : [];
        }

        if (!Directory.Exists(inputPath))
            return [];

        return Directory
            .EnumerateFiles(inputPath, "*", new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true
            })
            .Where(path =>
                Path.GetExtension(path).Equals($".{extension}", StringComparison.OrdinalIgnoreCase) &&
                !Path.GetFileName(path).StartsWith("~$"))
            .ToArray();
    }
}