using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using LightNovelTranslator.Core;
using Microsoft.AspNetCore.Mvc;

namespace LightNovelTranslator.Api.Controllers;

public class FileController : BaseController
{
    const string TEMP_FOLDER = "LightNovelTranslator";
    const string INPUT_FOLDER = "input";
    const string OUTPUT_FOLDER = "output";
    const string JOBS_FOLDER = "jobs";
    const string JOB_FILE = "job.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    [HttpGet("output")]
    public IActionResult GetOutput()
    {
        try
        {
            string fileName = "";
            string arguments = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = "powershell.exe";
                arguments =
                    "-NoProfile -STA -Command \"" +
                    "Add-Type -AssemblyName System.Windows.Forms; " +
                    "$form = New-Object System.Windows.Forms.Form; " +
                    "$form.TopMost = $true; " +
                    "$form.WindowState = 'Minimized'; " +
                    "$form.ShowInTaskbar = $false; " +
                    "$form.Show(); " +
                    "$form.Activate(); " +
                    "$dialog = New-Object System.Windows.Forms.FolderBrowserDialog; " +
                    "$dialog.Description = 'Wybierz folder zapisu nowelki'; " +
                    "if ($dialog.ShowDialog($form) -eq [System.Windows.Forms.DialogResult]::OK) { " +
                    "Write-Output $dialog.SelectedPath " +
                    "} " +
                    "$form.Close();\"";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // macOS używa wbudowanego AppleScript (osascript) do wywołania Findera
                fileName = "osascript";
                arguments = "-e \"POSIX path of (choose folder with prompt \\\"Wybierz folder zapisu nowelki:\\\")\"";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Na Linuksie (np. Twoim Manjaro) zenity lub kdialog to standard
                // Możesz sprawdzić, co jest dostępne, lub założyć zenity jako uniwersalny standard
                fileName = "zenity";
                arguments = "--file-selection --directory --title=\"Wybierz folder zapisu nowelki\"";
            }
            else
            {
                return StatusCode(500, "Niewspierany system operacyjny.");
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return StatusCode(500, "Nie udało się uruchomić systemowego okna wyboru.");
            }

            string selectedPath = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            
            var error = process.StandardError.ReadToEnd().Trim();

            if (process.ExitCode != 0)
            {
                return StatusCode(500, error);
            }
            // Specyficzny fix dla macOS (w przypadku kliknięcia Anuluj/Cancel AppleScript rzuca błąd na standard error)
            if (process.ExitCode != 0 || string.IsNullOrEmpty(selectedPath) || selectedPath.Contains("User canceled"))
            {
                return NoContent(); // Kod 204 - użytkownik zamknął okno bez wyboru
            }

            return Ok(new { path = selectedPath });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Błąd cross-platformowego okna dialogowego: {ex.Message}");
        }
    }
    
    [HttpPost("resolve")]
    public IActionResult Resolve([FromBody] FileName data)
    {
        var results = new List<FileNameResult>();

        var allDocx = Helper.GetDocumentsPaths(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "docx");

        foreach (var file in data.Files)
        {
            var path = allDocx.FirstOrDefault(path =>
                string.Equals(
                    Path.GetFileName(path),
                    file,
                    StringComparison.OrdinalIgnoreCase));

            if (path is not null)
            {
                results.Add(new FileNameResult
                {
                    File = file,
                    Path = path
                });
            }
        }

        return Ok(results);
    }
    
    
    [DisableRequestSizeLimit]
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] List<IFormFile> files, [FromForm] string? jobId)
    {
        jobId ??= Guid.NewGuid().ToString("N");
        
        var inputDir = Path.Combine(
            Path.GetTempPath(),
            TEMP_FOLDER,
            JOBS_FOLDER,
            jobId,
            INPUT_FOLDER);

        Directory.CreateDirectory(inputDir);

        GetJobJson(jobId, out var job);
        
        if (string.IsNullOrWhiteSpace(job.Name) && files.Count > 0)
        {
            job.Name = Path.GetFileNameWithoutExtension(files[0].FileName)
                .Replace("_", " ");
        }
        
        
        var savedFiles = new List<string>();

        foreach (var file in files)
        {
            var path = Path.Combine(inputDir, file.FileName);

            await using var stream = System.IO.File.Create(path);
            await file.CopyToAsync(stream);

            savedFiles.Add(path);
        }

        var newFiles = savedFiles
            .Select(path => new JobFile
            {
                FileName = Path.GetFileName(path),
                Path = path,
                Status = EStatus.Pending
            });
        
        job.InputPath = job.InputPath
            .Concat(newFiles)
            .DistinctBy(x => x.Path, StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        SaveJobJson(jobId, job);
        
        Console.WriteLine($"Files count: {files.Count} input dir: {inputDir}");

        foreach (var file in files)
        {
            Console.WriteLine($"File: {file.FileName}, Length: {file.Length}");
        }
        
        SaveJobJson(jobId, job);
        
        return Ok(new
        {
            jobId,
            inputDir,
            files = savedFiles.Select(path => new
            {
                fileName = Path.GetFileName(path),
                path
            })
        });
    }


    private void GetJobJson(string jobId, out Job job)
    {
        var jobPath = GetJobJsonPath(jobId);
        var jobDir = Path.GetDirectoryName(jobPath);

        if (!string.IsNullOrWhiteSpace(jobDir))
            Directory.CreateDirectory(jobDir);

        if (!System.IO.File.Exists(jobPath))
        {
            job = new Job
            {
                Id = jobId,
            };

            SaveJobJson(jobId, job);
            return;
        }

        var json = System.IO.File.ReadAllText(jobPath);
        job = JsonSerializer.Deserialize<Job>(json, JsonOptions)
              ?? throw new InvalidOperationException($"Nie udało się wczytać pliku job JSON: {jobPath}");
    }

    private void SaveJobJson(string jobId, Job job)
    {
        var jobPath = GetJobJsonPath(jobId);
        var jobDir = Path.GetDirectoryName(jobPath);

        if (!string.IsNullOrWhiteSpace(jobDir))
            Directory.CreateDirectory(jobDir);

        var json = JsonSerializer.Serialize(job, JsonOptions);
        System.IO.File.WriteAllText(jobPath, json);
    }

    private  string GetJobJsonPath(string jobId)
    {
        return Path.Combine(
            Path.GetTempPath(),
            TEMP_FOLDER,
            JOBS_FOLDER,
            jobId,
            JOB_FILE);
    }
    
    public sealed class FileName
    {
        public List<string> Files { get; set; } = [];
    }

    public sealed class FileNameResult
    {
        public string File { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
