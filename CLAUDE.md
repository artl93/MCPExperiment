# Microsoft.Extensions.MCP Guidelines

## Build Commands
- `dotnet build` - Build the project
- `dotnet build --configuration Release` - Build in release mode
- `dotnet test` - Run all tests
- `dotnet test --filter "FullyQualifiedName=Namespace.TestClass.TestMethod"` - Run single test

## Code Style Guidelines
- **Naming**: PascalCase for types/members, camelCase for parameters/variables
- **Formatting**: 4-space indentation, braces on new lines
- **Imports**: Group System imports first, then Microsoft, then others alphabetically
- **Nullable**: Project has nullable reference types enabled - use `?` for nullable types
- **Types**: Prefer explicit types over `var` for clarity
- **Error Handling**: Use exceptions for exceptional conditions, null checks with guard clauses
- **Comments**: XML documentation on public APIs
- **Design Patterns**: Follow standard JSON-RPC protocols and Microsoft Extension patterns

## Project Structure
This library implements a Model Context Protocol (MCP) for client-server communication with AI models.