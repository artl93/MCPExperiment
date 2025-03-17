using MCPExperiment.Annotations;
using MCPExperiment.Models;
using MCPExperiment.Server.Attributes;

namespace MCPExperiment.TestApp
{
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

    // Tool implementations with attributes
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