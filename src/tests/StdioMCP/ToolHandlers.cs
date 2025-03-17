using System.Threading.Tasks;
using MCPExperiment.Annotations;
using MCPExperiment.Models;
using MCPExperiment.Server.Attributes;
using MCPExperiment.TestApp;

namespace MCPExperiment.TestApp.StdioMCP
{
    // Tool handler implementations with attributes for discovery
    public class StdioWeatherHandler
    {
        [Tool(Name = "getWeather", Description = "Gets the weather for a specific location")]
        public async Task<WeatherResponse> HandleRequest(WeatherRequest request)
        {
            return await SharedImplementations.GetWeatherAsync(request);
        }
    }

    public class StdioCalculatorHandler
    {
        [Tool(Name = "calculate", Description = "Performs a calculation")]
        public int HandleRequest(CalculatorRequest request)
        {
            return SharedImplementations.CalculateResult(request);
        }
    }

    public class StdioGreetingHandler
    {
        [Prompt(Name = "greeting", Description = "Generates a greeting message", Category = "Messaging")]
        public async Task<PromptMessage[]> HandleRequest(GreetingRequest request)
        {
            return await SharedImplementations.GenerateGreetingAsync(request);
        }
    }
}