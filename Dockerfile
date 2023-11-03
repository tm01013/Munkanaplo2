# Use the .NET 7.0 SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:7.0.401 AS build-env
WORKDIR /app

# Copy the project file and restore dependencies
#COPY *.csproj ./
COPY . ./
RUN dotnet restore Munkanaplo2.csproj

# Copy the remaining files and build the app

RUN dotnet publish Munkanaplo2.csproj -c Release -o out

# Use the .NET 7.0 runtime image as the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Munkanaplo2.dll"]