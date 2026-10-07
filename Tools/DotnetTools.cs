using System.ComponentModel;
using System.Diagnostics;

namespace DotnetAgentDemo.Tools;

public sealed class DotnetTools
{
    [Description("Gets the installed .NET SDK version.")]
    public async Task<string> GetDotnetVersion()
    {
        Console.WriteLine("\n[TOOL CALLED] GetDotnetVersion");
        Console.WriteLine("[TOOL] Iniciando execução...");

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.StartInfo.ArgumentList.Add("--version");

        if (!process.Start())
        {
            const string unavailable = "Não foi possível iniciar o comando dotnet.";
            Console.WriteLine($"[TOOL RESULT] {unavailable}");
            return unavailable;
        }

        var standardOutputTask = process.StandardOutput.ReadToEndAsync();
        var standardErrorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        var version = (await standardOutputTask).Trim();
        _ = await standardErrorTask;

        var result = process.ExitCode == 0 && !string.IsNullOrWhiteSpace(version)
            ? $"Installed .NET SDK: {version}"
            : "Não foi possível determinar a versão do .NET SDK.";

        Console.WriteLine($"[TOOL RESULT] {result}");
        return result;
    }
}