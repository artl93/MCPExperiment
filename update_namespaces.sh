#!/bin/bash

# Update namespace declarations
find /Users/artleonard/src/Microsoft.Extensions.ModelContextProtocol/src/Microsoft.MCP -name "*.cs" -type f -exec sed -i '' 's/namespace Microsoft.Extensions.AI.MCP/namespace Microsoft.MCP/g' {} \;

# Update using statements
find /Users/artleonard/src/Microsoft.Extensions.ModelContextProtocol/src/Microsoft.MCP -name "*.cs" -type f -exec sed -i '' 's/using Microsoft.Extensions.AI.MCP/using Microsoft.MCP/g' {} \;

echo "Namespace updates completed."