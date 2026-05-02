using System.Diagnostics;

namespace GitUIClient.Services;

public class GitService : IGitService
{
    private readonly string _workingDirectory;

    public GitService(string workingDirectory = ".")
    {
        _workingDirectory = workingDirectory;
    }

    private string ExecuteGitCommand(string arguments)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            WorkingDirectory = _workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);
        
        if (process == null)
        {
            throw new Exception("Не вдалося запустити процес Git. Перевірте, чи встановлено Git.");
        }

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Помилка Git (код {process.ExitCode}):\n{error}");
        }

        return output;
    }

    public string GetStatus() => ExecuteGitCommand("status --short");
    
    public string GetLog(int count = 5) => ExecuteGitCommand($"log --oneline -n {count}");
    
    public void Add(string path = ".") => ExecuteGitCommand($"add {path}");
    
    public string Commit(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Повідомлення коміту не може бути порожнім.");

        return ExecuteGitCommand($"commit -m \"{message}\"");
    }
}