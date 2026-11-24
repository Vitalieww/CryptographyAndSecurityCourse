using System.Diagnostics;

namespace OpenSslPkiManager.Services
{
    public class CommandResult
    {
        public int ExitCode { get; set; }
        public string StandardOutput { get; set; } = string.Empty;
        public string StandardError { get; set; } = string.Empty;
        public bool Success => ExitCode == 0;
        public string CombinedOutput => $"{StandardOutput}\n{StandardError}".Trim();
    }

    public static class OpenSslCommandRunner
    {
        public static CommandResult Run(string arguments, string? workingDir = null)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "openssl",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (!string.IsNullOrEmpty(workingDir))
            {
                startInfo.WorkingDirectory = workingDir;
            }

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return new CommandResult
            {
                ExitCode = process.ExitCode,
                StandardOutput = stdout,
                StandardError = stderr
            };
        }

        public static bool IsOpenSslAvailable(out string version)
        {
            try
            {
                var result = Run("version");
                if (result.Success)
                {
                    version = result.StandardOutput.Trim();
                    return true;
                }
            }
            catch
            {
                // Process execution exception
            }

            version = string.Empty;
            return false;
        }
    }
}
