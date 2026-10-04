# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore stage, copy csproj first then retore
COPY FileProcessing.Api/FileProcessing.Api.csproj FileProcessing.Api/
RUN dotnet restore FileProcessing.Api/FileProcessing.Api.csproj

COPY FileProcessing.Api/ FileProcessing.Api/
RUN dotnet publish FileProcessing.Api/FileProcessing.Api.csproj -c Release -o /app/publish --no-restore 

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .
# set ENV Port to list to 8085  
ENV ASPNETCORE_HTTP_PORTS=8085

# Expose port
EXPOSE 8085
# run as non user 
USER $APP_UID

ENTRYPOINT ["dotnet", "FileProcessing.Api.dll"]
