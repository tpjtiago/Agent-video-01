namespace DotnetAgentDemo.Models;

public sealed record ProjectInfo(
    string Name,
    string TargetFramework,
    string ProjectType);