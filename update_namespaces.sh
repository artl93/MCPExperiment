#!/bin/bash

# Update namespace declarations
find /Users/artleonard/src/Microsoft.Extensions.ModelContextProtocol/src/MCPExperiment -name "*.cs" -type f -exec sed -i '' 's/namespace Microsoft.Extensions.AI.MCP/namespace MCPExperiment/g' {} \;

# Update using statements
find /Users/artleonard/src/Microsoft.Extensions.ModelContextProtocol/src/MCPExperiment -name "*.cs" -type f -exec sed -i '' 's/using Microsoft.Extensions.AI.MCP/using MCPExperiment/g' {} \;

echo "Namespace updates completed."