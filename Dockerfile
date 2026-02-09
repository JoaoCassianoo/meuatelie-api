# Etapa base: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Etapa build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Atelie.Api/Atelie.Api.csproj", "Atelie.Api/"]
RUN dotnet restore "Atelie.Api/Atelie.Api.csproj"
COPY . .
WORKDIR "/src/Atelie.Api"
RUN dotnet build "Atelie.Api.csproj" -c Release -o /app/build

# Etapa publish
FROM build AS publish
RUN dotnet publish "Atelie.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Atelie.Api.dll"]
