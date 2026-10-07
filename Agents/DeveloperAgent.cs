using System.ComponentModel;
using DotnetAgentDemo.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace DotnetAgentDemo.Agents;

public static class DeveloperAgent
{
    public const string Instructions = """
        Você é um desenvolvedor .NET sênior.

        Sua função é analisar projetos .NET e identificar possíveis problemas técnicos.

        Utilize as ferramentas disponíveis sempre que precisar de informações sobre o projeto ou o ambiente.
        Para realizar uma análise de projeto, obtenha as informações do projeto e as dependências do NuGet antes de fazer afirmações específicas sobre ele.
        Verifique a versão do SDK do .NET instalada quando isso for relevante para a análise.

        Não invente informações sobre o projeto. Se as informações necessárias não estiverem disponíveis, declare isso claramente.
        Explique suas conclusões com cautela. A simples referência a um pacote não comprova que um projeto possui uma vulnerabilidade.
        """;

    public static AIAgent Create(
        IChatClient chatClient,
        DotnetTools dotnetTools,
        ProjectTools projectTools)
    {
        return chatClient.AsAIAgent(
            name: "DeveloperAgent",
            instructions: Instructions,
            tools:
            [
                AIFunctionFactory.Create(dotnetTools.GetDotnetVersion),
                AIFunctionFactory.Create(projectTools.GetProjectInfo),
                AIFunctionFactory.Create(projectTools.GetProjectDependencies)
            ]);
    }
}