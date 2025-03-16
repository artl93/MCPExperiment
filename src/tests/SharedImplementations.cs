using System;
using System.Threading.Tasks;
using Microsoft.MCP.Annotations;
using Microsoft.MCP.Models;
using Microsoft.MCP.TestApp;

namespace Microsoft.MCP.TestApp
{
    // Shared implementations for tools and prompts
    public static class SharedImplementations
    {
        // Tool implementations
        public static async Task<WeatherResponse> GetWeatherAsync(WeatherRequest request)
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

        public static int CalculateResult(CalculatorRequest request)
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
        public static async Task<PromptMessage[]> GenerateGreetingAsync(GreetingRequest request)
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
}