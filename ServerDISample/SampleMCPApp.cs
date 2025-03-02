using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI.MCP;
using Microsoft.Extensions.AI.MCP.Annotations;
using Microsoft.Extensions.AI.MCP.Models;
using Microsoft.Extensions.AI.MCP.Server;
using Microsoft.Extensions.AI.MCP.Server.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleMCPApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // If command-line argument "stdio" is provided, run as stdio server
            bool useStdio = args.Contains("--stdio");
            
            if (useStdio)
            {
                await RunAsStdioServer();
            }
            else
            {
                await RunAsWebServer();
            }
        }

        private static async Task RunAsStdioServer()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    // logging.AddFile("mcp-stdio.log"); // Note: This would require a file logging package
                })
                .ConfigureServices(services =>
                {
                    services.AddMCP(options =>
                    {
                        options.EnableStdioProtocol = true;
                    });
                })
                .Build();

            await host.RunMCPConsoleAsync();
        }

        private static async Task RunAsWebServer()
        {
            var builder = WebApplication.CreateBuilder();
            
            // Add services to the container
            builder.Services.AddMCP();
            
            var app = builder.Build();
            
            // Configure the HTTP pipeline
            app.UseMCP();
            
            // Map tools and prompts using the extension methods
            app.MapTool<WeatherRequest, WeatherResponse>(
                "getWeather",
                "Gets the weather for a specific location",
                async (request) => await GetWeatherAsync(request));
            
            app.MapSyncTool<CalculatorRequest, int>(
                "calculate",
                "Performs a calculation",
                (request) => CalculateResult(request));
            
            app.MapPrompt<GreetingRequest, PromptMessage[]>(
                "greeting",
                "Generates a greeting message",
                "Messaging",
                async (request) => await GenerateGreetingAsync(request));
            
            // Start the server
            await app.RunAsync();
        }

        // Tool implementations
        private static async Task<WeatherResponse> GetWeatherAsync(WeatherRequest request)
        {
            // Simulate an async operation
            await Task.Delay(100);
            
            var response = new WeatherResponse
            {
                Temperature = 72,
                Conditions = "Sunny",
                Location = request.Location
            };
            
            return response;
        }

        private static int CalculateResult(CalculatorRequest request)
        {
            return request.Operation switch
            {
                "add" => request.A + request.B,
                "subtract" => request.A - request.B,
                "multiply" => request.A * request.B,
                "divide" => request.B != 0 ? request.A / request.B : 0,
                _ => 0
            };
        }

        // Prompt implementations
        private static async Task<PromptMessage[]> GenerateGreetingAsync(GreetingRequest request)
        {
            // Simulate an async operation
            await Task.Delay(100);
            
            var timeOfDay = DateTime.Now.Hour switch
            {
                >= 5 and < 12 => "morning",
                >= 12 and < 17 => "afternoon",
                _ => "evening"
            };
            
            var formality = request.Formal ? "formal" : "casual";
            var greeting = formality == "formal" ? "Greetings" : "Hey";
            
            var message = $"{greeting} {request.Name}, good {timeOfDay}!";
            
            return new[] 
            {
                new PromptMessage 
                { 
                    Role = "system",
                    Content = new TextContent { Text = message }
                }
            };
        }
    }

    // Data models for tools and prompts
    public class WeatherRequest
    {
        [ToolParameter(Description = "The location to get weather for", Required = true)]
        public string Location { get; set; } = string.Empty;

        [ToolParameter(Description = "The unit of temperature (celsius or fahrenheit)", Required = false)]
        public string Unit { get; set; } = "fahrenheit";
    }

    public class WeatherResponse
    {
        public double Temperature { get; set; }
        public string Conditions { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class CalculatorRequest
    {
        [ToolParameter(Description = "The first operand")]
        public int A { get; set; }

        [ToolParameter(Description = "The second operand")]
        public int B { get; set; }

        [ToolParameter(Description = "The operation to perform (add, subtract, multiply, divide)")]
        public string Operation { get; set; } = "add";
    }

    public class GreetingRequest
    {
        [PromptParameter(Description = "The name of the person to greet")]
        public string Name { get; set; } = string.Empty;

        [PromptParameter(Description = "Whether to use formal language", Required = false)]
        public bool Formal { get; set; } = false;
    }

    // Alternative example with attributes on methods
    public class ToolExamples
    {
        [Tool(Name = "stringLength", Description = "Counts the number of characters in a string")]
        public int GetStringLength([ToolParameter(Description = "The string to count")] string text)
        {
            return text?.Length ?? 0;
        }

        [Tool(Description = "Converts a string to uppercase")]
        public string ToUpperCase([ToolParameter] string text)
        {
            return text?.ToUpper() ?? string.Empty;
        }

        [Prompt(Name = "askQuestion", Description = "Generates a message asking a question", Category = "Conversation")]
        public PromptMessage[] GenerateQuestion(
            [PromptParameter(Description = "The topic of the question")] string topic,
            [PromptParameter(Description = "The difficulty level", Required = false)] string difficulty = "medium")
        {
            return new[] 
            {
                new PromptMessage 
                { 
                    Role = "system",
                    Content = new TextContent { Text = $"Please ask a {difficulty} question about {topic}." }
                }
            };
        }
    }
}