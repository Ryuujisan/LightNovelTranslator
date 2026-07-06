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
}