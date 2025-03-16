using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.MCP.Capabilities;
using Microsoft.MCP.Models;
using Microsoft.MCP.Server.Attributes;
using Microsoft.MCP.Server.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

namespace Microsoft.MCP.Server
{
    /// <summary>
    /// Extension methods for routing MCP tools and prompts.
    /// </summary>
    public static class MCPRoutingExtensions
    {
        private static readonly Dictionary<string, ToolDefinition> _tools = new Dictionary<string, ToolDefinition>();
        private static readonly Dictionary<string, PromptDefinition> _prompts = new Dictionary<string, PromptDefinition>();

        /// <summary>
        /// Maps a tool to the MCP server.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="app">The web application.</param>
        /// <param name="name">The name of the tool.</param>
        /// <param name="description">The description of the tool.</param>
        /// <param name="handler">The handler delegate.</param>
        /// <returns>The web application.</returns>
        public static IHost MapTool<TRequest, TResult>(
            this IHost app,
            string name,
            string description,
            Func<TRequest, Task<TResult>> handler)
        {
            var toolDefinition = new ToolDefinition
            {
                Name = name,
                Description = description,
                Handler = handler,
                IsSync = false,
                Parameters = CreateParametersFromType(typeof(TRequest))
            };

            _tools[name] = toolDefinition;
            UpdateServerCapabilities(app.Services);

            return app;
        }

        /// <summary>
        /// Maps a synchronous tool to the MCP server.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="app">The web application.</param>
        /// <param name="name">The name of the tool.</param>
        /// <param name="description">The description of the tool.</param>
        /// <param name="handler">The handler delegate.</param>
        /// <returns>The web application.</returns>
        public static IHost MapSyncTool<TRequest, TResult>(
            this IHost app,
            string name,
            string description,
            Func<TRequest, TResult> handler)
        {
            var toolDefinition = new ToolDefinition
            {
                Name = name,
                Description = description,
                Handler = handler,
                IsSync = true,
                Parameters = CreateParametersFromType(typeof(TRequest))
            };

            _tools[name] = toolDefinition;
            UpdateServerCapabilities(app.Services);

            return app;
        }

        /// <summary>
        /// Maps a tool to the MCP server using method attributes.
        /// </summary>
        /// <param name="app">The web application.</param>
        /// <param name="method">The method to map.</param>
        /// <returns>The web application.</returns>
        public static IHost MapTool(this IHost app, MethodInfo method)
        {
            var toolAttr = method.GetCustomAttribute<ToolAttribute>();
            if (toolAttr == null)
            {
                throw new ArgumentException("Method does not have a Tool attribute", nameof(method));
            }

            var name = toolAttr.Name ?? method.Name;
            var parameters = method.GetParameters();
            var paramDefinitions = new Dictionary<string, ToolParameterDefinition>();

            foreach (var parameter in parameters)
            {
                var paramAttr = parameter.GetCustomAttribute<ToolParameterAttribute>();
                var description = paramAttr?.Description ?? parameter.Name ?? "Parameter";
                var required = paramAttr?.Required ?? true;

                paramDefinitions[parameter.Name ?? "param"] = new ToolParameterDefinition
                {
                    Description = description,
                    Type = parameter.ParameterType.Name,
                    Required = required
                };
            }

            Delegate handler = CreateDelegateForMethod(method);

            var toolDefinition = new ToolDefinition
            {
                Name = name,
                Description = toolAttr.Description,
                Handler = handler,
                IsSync = toolAttr.IsSync,
                Parameters = paramDefinitions
            };

            _tools[name] = toolDefinition;
            UpdateServerCapabilities(app.Services);

            return app;
        }

        /// <summary>
        /// Maps a prompt to the MCP server.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="app">The web application.</param>
        /// <param name="name">The name of the prompt.</param>
        /// <param name="description">The description of the prompt.</param>
        /// <param name="category">The category of the prompt.</param>
        /// <param name="handler">The handler delegate.</param>
        /// <returns>The web application.</returns>
        public static IHost MapPrompt<TRequest, TResult>(
            this IHost app,
            string name,
            string description,
            string category,
            Func<TRequest, Task<TResult>> handler)
        {
            var promptDefinition = new PromptDefinition
            {
                Name = name,
                Description = description,
                Category = category,
                Handler = handler,
                Parameters = CreatePromptParametersFromType(typeof(TRequest))
            };

            _prompts[name] = promptDefinition;
            UpdateServerCapabilities(app.Services);

            return app;
        }

        /// <summary>
        /// Maps a prompt to the MCP server using method attributes.
        /// </summary>
        /// <param name="app">The web application.</param>
        /// <param name="method">The method to map.</param>
        /// <returns>The web application.</returns>
        public static IHost MapPrompt(this IHost app, MethodInfo method)
        {
            var promptAttr = method.GetCustomAttribute<PromptAttribute>();
            if (promptAttr == null)
            {
                throw new ArgumentException("Method does not have a Prompt attribute", nameof(method));
            }

            var name = promptAttr.Name ?? method.Name;
            var parameters = method.GetParameters();
            var paramDefinitions = new Dictionary<string, PromptParameterDefinition>();

            foreach (var parameter in parameters)
            {
                var paramAttr = parameter.GetCustomAttribute<PromptParameterAttribute>();
                var description = paramAttr?.Description ?? parameter.Name ?? "Parameter";
                var required = paramAttr?.Required ?? true;
                var defaultValue = paramAttr?.DefaultValue;

                paramDefinitions[parameter.Name ?? "param"] = new PromptParameterDefinition
                {
                    Description = description,
                    Type = parameter.ParameterType.Name,
                    Required = required,
                    DefaultValue = defaultValue
                };
            }

            Delegate handler = CreateDelegateForMethod(method);

            var promptDefinition = new PromptDefinition
            {
                Name = name,
                Description = promptAttr.Description,
                Category = promptAttr.Category,
                Handler = handler,
                Parameters = paramDefinitions
            };

            _prompts[name] = promptDefinition;
            UpdateServerCapabilities(app.Services);

            return app;
        }

        /// <summary>
        /// Gets all registered tools.
        /// </summary>
        /// <returns>The collection of tool definitions.</returns>
        public static IReadOnlyDictionary<string, ToolDefinition> GetTools()
        {
            return _tools;
        }

        /// <summary>
        /// Gets all registered prompts.
        /// </summary>
        /// <returns>The collection of prompt definitions.</returns>
        public static IReadOnlyDictionary<string, PromptDefinition> GetPrompts()
        {
            return _prompts;
        }

        private static Dictionary<string, ToolParameterDefinition> CreateParametersFromType(Type type)
        {
            var result = new Dictionary<string, ToolParameterDefinition>();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                result[property.Name] = new ToolParameterDefinition
                {
                    Description = property.Name,
                    Type = property.PropertyType.Name,
                    Required = !IsNullableType(property.PropertyType)
                };
            }

            return result;
        }

        private static Dictionary<string, PromptParameterDefinition> CreatePromptParametersFromType(Type type)
        {
            var result = new Dictionary<string, PromptParameterDefinition>();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                result[property.Name] = new PromptParameterDefinition
                {
                    Description = property.Name,
                    Type = property.PropertyType.Name,
                    Required = !IsNullableType(property.PropertyType),
                    DefaultValue = null
                };
            }

            return result;
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static Delegate CreateDelegateForMethod(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            var returnType = method.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // Async method returning Task<T>
                var resultType = returnType.GetGenericArguments()[0];
                var delegateType = typeof(Func<,>).MakeGenericType(parameters[0].ParameterType, returnType);
                return Delegate.CreateDelegate(delegateType, method);
            }
            else if (returnType == typeof(Task))
            {
                // Async method returning Task
                var delegateType = typeof(Func<,>).MakeGenericType(parameters[0].ParameterType, returnType);
                return Delegate.CreateDelegate(delegateType, method);
            }
            else
            {
                // Sync method
                var delegateType = typeof(Func<,>).MakeGenericType(parameters[0].ParameterType, returnType);
                return Delegate.CreateDelegate(delegateType, method);
            }
        }

        private static void UpdateServerCapabilities(IServiceProvider services)
        {
            try
            {
                var options = services.GetRequiredService<IOptions<MCPServerOptions>>();
                
                if (options.Value.ServerCapabilities.Tools == null)
                {
                    options.Value.ServerCapabilities.Tools = new ServerToolsCapabilities();
                }
    
                if (options.Value.ServerCapabilities.Prompts == null)
                {
                    options.Value.ServerCapabilities.Prompts = new ServerPromptsCapabilities();
                }
                
                // Update the available tools and prompts in the capabilities
                if (options.Value.ServerCapabilities.Tools.AvailableTools != null)
                {
                    options.Value.ServerCapabilities.Tools.AvailableTools.Clear();
                    foreach (var tool in _tools.Values)
                    {
                        options.Value.ServerCapabilities.Tools.AvailableTools.Add(tool);
                    }
                }
                
                if (options.Value.ServerCapabilities.Prompts.AvailablePrompts != null)
                {
                    options.Value.ServerCapabilities.Prompts.AvailablePrompts.Clear();
                    foreach (var prompt in _prompts.Values)
                    {
                        options.Value.ServerCapabilities.Prompts.AvailablePrompts.Add(prompt);
                    }
                }
                
                // Try to get the server instance and directly update its capabilities if it exists
                try
                {
                    var server = services.GetService<IMCPServer>();
                    if (server != null)
                    {
                        // Server instance exists, it will use updated capabilities on next request
                        System.Diagnostics.Debug.WriteLine($"Updated MCP Server capabilities: {_tools.Count} tools, {_prompts.Count} prompts");
                        
                        // Send a notification that tools have changed
                        try 
                        {
                            // Use the new method to send tool list changed notification
                            server.SendToolListChangedNotification();
                            System.Diagnostics.Debug.WriteLine("Sent ToolListChanged notification to clients");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to send ToolListChanged notification: {ex.Message}");
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    // This is expected during startup before server is registered
                    // The server will pick up the capabilities from options when it's created
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating server capabilities: {ex.Message}");
            }
        }
    }
}