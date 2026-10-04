# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FileProcessing.Api/FileProcessing.Api.csproj FileProcessing.Api/
RUN dotnet restore FileProcessing.Api/FileProcessing.Api.csproj

COPY FileProcessing.Api/ FileProcessing.Api/
RUN dotnet publish FileProcessing.Api/FileProcessing.Api.csproj -c Release -o /app/publish --no-restore 

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .


# Expose port
EXPOSE 8085

ENTRYPOINT ["dotnet", "FileProcessing.Api.dll"]
