# Contributing to MCP Experiment
Thank you for your interest in contributing to MCP Experiment! This document provides guidelines and instructions for contributing to this project.

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## How to Contribute

### Reporting Issues

- Check if the issue has already been reported
- Use the issue template if available
- Include as much detail as possible:
  - Steps to reproduce
  - Expected behavior
  - Actual behavior
  - Version/commit information
  - Environment details

### Pull Requests

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Pull Request Guidelines

- Update the README.md with details of changes if appropriate
- Update any documentation that is affected by your code changes
- The PR should work for .NET 9.0 and later
- Follow the existing code style and formatting conventions
- Include appropriate tests
- Link any relevant issues in the PR description

## Development Setup

1. Install .NET 9.0 SDK or later
2. Clone the repository
3. Build the solution with `dotnet build`
4. Run tests with `dotnet test`

## Coding Style Guidelines

- **Naming**: PascalCase for types/members, camelCase for parameters/variables
- **Formatting**: 4-space indentation, braces on new lines
- **Imports**: Group System imports first, then Microsoft, then others alphabetically
- **Nullable**: Project has nullable reference types enabled - use `?` for nullable types
- **Types**: Prefer explicit types over `var` for clarity
- **Error Handling**: Use exceptions for exceptional conditions, null checks with guard clauses
- **Comments**: XML documentation on public APIs
- **Design Patterns**: Follow standard JSON-RPC protocols and Microsoft Extension patterns

## Testing

- Write unit tests for new features and bug fixes
- Ensure all tests pass before submitting a PR
- Test both HTTP and stdio transport implementations when applicable

## Documentation

- Update XML documentation for public APIs
- Keep code samples in README up to date
- Document any breaking changes

## Release Process

The maintainers will handle the release process, including:
- Version bumping
- Changelog updates
- NuGet package publishing

## License

By contributing to Microsoft.Extensions.ModelContextProtocol, you agree that your contributions will be licensed under the project's MIT license.
