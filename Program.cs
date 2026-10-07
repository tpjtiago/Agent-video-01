using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using DotnetAgentDemo.Agents;
using DotnetAgentDemo.Tools;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

var apiKey = configuration["AI:ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("API key ausente. Configure AI:ApiKey com dotnet user-secrets.");
    return;
}

var model = configuration["AI:Model"] ?? "gpt-4o-mini";
var chatClient = new OpenAIClient(apiKey)
    .GetChatClient(model)
    .AsIChatClient();

var dotnetTools = new DotnetTools();
var projectTools = new ProjectTools();
var agent = DeveloperAgent.Create(chatClient, dotnetTools, projectTools);
var agentSession = await agent.CreateSessionAsync();

Console.WriteLine("====================================");
Console.WriteLine("        .NET DEVELOPER AGENT");
Console.WriteLine("====================================");
Console.WriteLine($"Modelo: {model}");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 - Perguntar ao LLM");
    Console.WriteLine("2 - Perguntar ao AI Agent");
    Console.WriteLine("exit - Encerrar");
    Console.Write("Selecione: ");

    var mode = Console.ReadLine()?.Trim();
    if (string.Equals(mode, "exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (mode is not ("1" or "2"))
    {
        Console.WriteLine("Escolha 1, 2 ou exit.");
        continue;
    }

    while (true)
    {
        Console.Write("\nPergunta (exit volta ao menu):\n> ");
        var question = Console.ReadLine()?.Trim();

        if (string.Equals(question, "exit", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        if (string.IsNullOrWhiteSpace(question))
        {
            continue;
        }

        try
        {
            if (mode == "1")
            {
                Console.WriteLine("\n[LLM] Enviando pergunta diretamente ao modelo...\n");
                var response = await chatClient.GetResponseAsync(question);
                Console.WriteLine($"LLM:\n{response.Text}");
            }
            else
            {
                Console.WriteLine("\n[AGENT] Analisando a pergunta...\n");
                var response = await agent.RunAsync(question, agentSession);
                Console.WriteLine($"\nAGENT:\n{response.Text}");
            }
        }
        catch (Exception)
        {
            Console.WriteLine("\nNão foi possível concluir a solicitação. Verifique a configuração do provedor e tente novamente.");
        }
    }
}

Console.WriteLine("Encerrado.");