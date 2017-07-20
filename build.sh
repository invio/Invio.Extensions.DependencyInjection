#!/usr/bin/env bash

#exit if any command fails
set -e

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

dotnet restore ./src/Invio.Extensions.DependencyInjection
dotnet restore ./test/Invio.Extensions.DependencyInjection.Tests

dotnet test ./test/Invio.Extensions.DependencyInjection.Tests/Invio.Extensions.DependencyInjection.Tests.csproj -c Release -f netcoreapp1.0

dotnet pack ./src/Invio.Extensions.DependencyInjection -c Release -o ../../artifacts
