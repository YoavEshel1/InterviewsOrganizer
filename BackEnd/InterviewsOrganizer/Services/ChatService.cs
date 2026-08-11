using Azure.Identity;
using InterviewsOrganizer.Services.Interfaces;

using Azure.AI.Projects;
using Azure.AI.Projects.Agents;
using Azure.AI.Extensions.OpenAI;
using Azure.Identity;
using OpenAI.Responses;

namespace InterviewsOrganizer.Services
{
    public class ChatService:IChatService       
    {
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates
        public async Task<string> Chat(string userInput)
        {
            const string endpoint = "https://azure-try2-resource.services.ai.azure.com/api/projects/azure-try2";
            const string agentName = "computing-historian";
            const string agentVersion = "2";

            // Connect to your project using the endpoint from your project page
            // The AzureCliCredential will use your logged-in Azure CLI identity, make sure to run `az login` first
            AIProjectClient projectClient = new(endpoint: new Uri(endpoint), tokenProvider: new DefaultAzureCredential());

            AgentReference agentReference = new(name: agentName, version: agentVersion);
            ProjectResponsesClient responseClient = projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgent(agentReference);
            // Use the agent to generate a response
            ResponseResult response = responseClient.CreateResponse(
                userInput
            );
            return response.GetOutputText();
        }
#pragma warning restore OPENAI001
    }
}
