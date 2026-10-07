using System.ComponentModel;
using System.Xml.Linq;
using DotnetAgentDemo.Models;

namespace DotnetAgentDemo.Tools;

public sealed class ProjectTools
{
    private static readonly string ProjectFilePath = Path.Combine(
        AppContext.BaseDirectory,
        "SampleProject",
        "SampleProject.csproj");

    [Description("Gets basic information about the .NET project being analyzed.")]
    public ProjectInfo GetProjectInfo()
    {
        Console.WriteLine("\n[TOOL CALLED] GetProjectInfo");
        Console.WriteLine("[TOOL] Lendo o arquivo SampleProject.csproj...");

        var document = XDocument.Load(ProjectFilePath);
        var properties = document
            .Descendants()
            .Where(element => element.Name.LocalName == "PropertyGroup")
            .SelectMany(group => group.Elements())
            .ToDictionary(element => element.Name.LocalName, element => element.Value);

        var outputType = properties.GetValueOrDefault("OutputType", "Library");
        var projectType = outputType.Equals("Exe", StringComparison.OrdinalIgnoreCase)
            ? "Console application"
            : "Class library";
        var projectInfo = new ProjectInfo(
            Path.GetFileNameWithoutExtension(ProjectFilePath),
            properties.GetValueOrDefault("TargetFramework", "Not specified"),
            projectType);

        Console.WriteLine(
            $"[TOOL RESULT] {projectInfo.Name} | {projectInfo.TargetFramework} | {projectInfo.ProjectType}");
        return projectInfo;
    }

    [Description("Gets the NuGet dependencies used by the .NET project.")]
    public string[] GetProjectDependencies()
    {
        Console.WriteLine("\n[TOOL CALLED] GetProjectDependencies");
        Console.WriteLine("[TOOL] Lendo os PackageReferences do projeto...");

        var document = XDocument.Load(ProjectFilePath);
        var dependencies = document
            .Descendants()
            .Where(element => element.Name.LocalName == "PackageReference")
            .Select(element =>
            {
                var name = (string?)element.Attribute("Include")
                    ?? (string?)element.Attribute("Update")
                    ?? "Unknown package";
                var version = (string?)element.Attribute("Version")
                    ?? element.Elements().FirstOrDefault(child => child.Name.LocalName == "Version")?.Value
                    ?? "version not specified";

                return $"{name} {version}";
            })
            .ToArray();

        Console.WriteLine($"[TOOL RESULT] {dependencies.Length} dependência(s): {string.Join(", ", dependencies)}");
        return dependencies;
    }
}