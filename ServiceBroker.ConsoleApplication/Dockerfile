# Use the official .NET SDK image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the project files into the container
COPY . ./

# Publish the application
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory in the final container
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/out ./

# Start the application
ENTRYPOINT ["dotnet", "ServiceBroker.Console.dll"]
