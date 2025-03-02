using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.AI.MCP.Annotations;
using Microsoft.Extensions.AI.MCP.Messages;
using Microsoft.Extensions.AI.MCP.Models;
using Xunit;

namespace Microsoft.Extensions.MCP.Tests
{
    public class MCPSchemaTests
    {
        // Helper method to load JSON files from the Samples directory
        private string LoadJsonSample(string sampleFileName)
        {
            try
            {
                // Try to load from the output directory first
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Samples", sampleFileName);
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }

                // If that fails, try relative to the executing assembly's location
                var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = Path.Combine(assemblyLocation!, "Samples", sampleFileName);
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }

                throw new FileNotFoundException($"Could not find sample file: {sampleFileName}", sampleFileName);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading JSON sample {sampleFileName}: {ex}");
                throw;
            }
        }
        [Fact]
        public void TestAudioContentSerialization()
        {
            // Arrange
            var audioContent = new AudioContent
            {
                Type = "audio",
                Data = "SGVsbG8gV29ybGQh", // Base64 encoded "Hello World!"
                MimeType = "audio/mp3"
            };

            // Act
            var json = JsonSerializer.Serialize(audioContent);
            var deserialized = JsonSerializer.Deserialize<AudioContent>(json);

            // Assert
            Assert.Equal("audio", deserialized?.Type);
            Assert.Equal("SGVsbG8gV29ybGQh", deserialized?.Data);
            Assert.Equal("audio/mp3", deserialized?.MimeType);
        }

        [Fact]
        public void TestPromptMessageWithAudioContent()
        {
            // Arrange
            var promptMessage = new PromptMessage
            {
                Role = "user",
                Content = new AudioContent
                {
                    Type = "audio",
                    Data = "SGVsbG8gV29ybGQh", // Base64 encoded "Hello World!"
                    MimeType = "audio/mp3"
                }
            };

            // Act - this would normally use a special converter in production
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(promptMessage, options);

            // Assert
            Assert.Contains("\"role\":\"user\"", json);
            Assert.Contains("\"type\":\"audio\"", json);
            Assert.Contains("\"data\":\"SGVsbG8gV29ybGQh\"", json);
            Assert.Contains("\"mimeType\":\"audio/mp3\"", json);
        }

        [Fact]
        public void TestGetPromptResultWithMixedContentTypes()
        {
            // Arrange
            var getPromptResult = new GetPromptResult
            {
                Description = "Test prompt with mixed content types",
                Messages = new List<PromptMessage>
                {
                    new PromptMessage
                    {
                        Role = "assistant",
                        Content = new TextContent
                        {
                            Type = "text",
                            Text = "This is a test message"
                        }
                    },
                    new PromptMessage
                    {
                        Role = "user",
                        Content = new AudioContent
                        {
                            Type = "audio",
                            Data = "SGVsbG8gV29ybGQh",
                            MimeType = "audio/mp3"
                        }
                    }
                }
            };

            // Act
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(getPromptResult, options);

            // Assert
            Assert.Contains("\"description\":\"Test prompt with mixed content types\"", json);
            Assert.Contains("\"role\":\"assistant\"", json);
            Assert.Contains("\"type\":\"text\"", json);
            Assert.Contains("\"text\":\"This is a test message\"", json);
            Assert.Contains("\"role\":\"user\"", json);
            Assert.Contains("\"type\":\"audio\"", json);
        }

        [Fact]
        public void TestCallToolResultWithMultipleContentTypes()
        {
            // Arrange
            var callToolResult = new CallToolResult
            {
                IsError = false,
                Content = new List<object>
                {
                    new TextContent
                    {
                        Type = "text",
                        Text = "This is a test result"
                    },
                    new AudioContent
                    {
                        Type = "audio",
                        Data = "SGVsbG8gV29ybGQh",
                        MimeType = "audio/mp3"
                    }
                }
            };

            // Act
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(callToolResult, options);

            // Assert
            Assert.Contains("\"isError\":false", json);
            Assert.Contains("\"type\":\"text\"", json);
            Assert.Contains("\"text\":\"This is a test result\"", json);
            Assert.Contains("\"type\":\"audio\"", json);
            Assert.Contains("\"data\":\"SGVsbG8gV29ybGQh\"", json);
            Assert.Contains("\"mimeType\":\"audio/mp3\"", json);
        }

        [Fact]
        public void ValidateJsonSamples()
        {
            // Define the sample files to validate
            var sampleFiles = new[]
            {
                "audio-content.json",
                "prompt-message-audio.json",
                "get-prompt-result.json",
                "call-tool-result.json"
            };

            // Load and parse each sample file
            foreach (var sampleFile in sampleFiles)
            {
                var json = LoadJsonSample(sampleFile);
                var jsonDoc = JsonDocument.Parse(json);
                Assert.NotNull(jsonDoc);
            }
        }

        [Fact]
        public void DeserializeAudioContentFromJson()
        {
            // Load the JSON sample
            var json = LoadJsonSample("audio-content.json");
            
            // Deserialize into our model
            var audioContent = JsonSerializer.Deserialize<AudioContent>(json, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            // Validate the deserialized object
            Assert.NotNull(audioContent);
            Assert.Equal("audio", audioContent.Type);
            Assert.Equal("SGVsbG8gV29ybGQh", audioContent.Data);
            Assert.Equal("audio/mp3", audioContent.MimeType);
        }

        [Fact]
        public void DeserializePromptMessageWithAudio()
        {
            // Need a custom deserializer for production use
            // This test just validates the JSON structure
            var json = LoadJsonSample("prompt-message-audio.json");
            var jsonDoc = JsonDocument.Parse(json);
            
            // Validate the structure manually
            var root = jsonDoc.RootElement;
            Assert.Equal("user", root.GetProperty("role").GetString());
            
            var content = root.GetProperty("content");
            Assert.Equal("audio", content.GetProperty("type").GetString());
            Assert.Equal("SGVsbG8gV29ybGQh", content.GetProperty("data").GetString());
            Assert.Equal("audio/mp3", content.GetProperty("mimeType").GetString());
        }

        [Fact]
        public void DeserializeGetPromptResultWithMixedContent()
        {
            // Need a custom deserializer for production use
            // This test just validates the JSON structure
            var json = LoadJsonSample("get-prompt-result.json");
            var jsonDoc = JsonDocument.Parse(json);
            
            // Validate the structure manually
            var root = jsonDoc.RootElement;
            Assert.Equal("Sample prompt with audio", root.GetProperty("description").GetString());
            
            var messages = root.GetProperty("messages");
            Assert.Equal(2, messages.GetArrayLength());
            
            // First message - text content
            var firstMessage = messages[0];
            Assert.Equal("assistant", firstMessage.GetProperty("role").GetString());
            var firstContent = firstMessage.GetProperty("content");
            Assert.Equal("text", firstContent.GetProperty("type").GetString());
            Assert.Equal("How can I help you today?", firstContent.GetProperty("text").GetString());
            
            // Second message - audio content
            var secondMessage = messages[1];
            Assert.Equal("user", secondMessage.GetProperty("role").GetString());
            var secondContent = secondMessage.GetProperty("content");
            Assert.Equal("audio", secondContent.GetProperty("type").GetString());
            Assert.Equal("SGVsbG8gV29ybGQh", secondContent.GetProperty("data").GetString());
            Assert.Equal("audio/mp3", secondContent.GetProperty("mimeType").GetString());
        }

        [Fact]
        public void DeserializeCallToolResultWithMultipleContentTypes()
        {
            // Need a custom deserializer for production use
            // This test just validates the JSON structure
            var json = LoadJsonSample("call-tool-result.json");
            var jsonDoc = JsonDocument.Parse(json);
            
            // Validate the structure manually
            var root = jsonDoc.RootElement;
            Assert.False(root.GetProperty("isError").GetBoolean());
            
            var content = root.GetProperty("content");
            Assert.Equal(2, content.GetArrayLength());
            
            // First content item - text
            var firstContent = content[0];
            Assert.Equal("text", firstContent.GetProperty("type").GetString());
            Assert.Equal("The audio has been processed.", firstContent.GetProperty("text").GetString());
            
            // Second content item - audio
            var secondContent = content[1];
            Assert.Equal("audio", secondContent.GetProperty("type").GetString());
            Assert.Equal("UHJvY2Vzc2VkIEF1ZGlv", secondContent.GetProperty("data").GetString());
            Assert.Equal("audio/wav", secondContent.GetProperty("mimeType").GetString());
        }
    }
}