# ============================================
# STAGE 1: BUILD
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar todos los .csproj primero (para aprovechar caché de capas)
COPY ["TaskFlow.Api/TaskFlow.Api.csproj", "TaskFlow.Api/"]
COPY ["TaskFlow.Application/TaskFlow.Application.csproj", "TaskFlow.Application/"]
COPY ["TaskFlow.Domain/TaskFlow.Domain.csproj", "TaskFlow.Domain/"]
COPY ["TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj", "TaskFlow.Infrastructure/"]

# Restaurar dependencias (se cachea si los .csproj no cambian)
RUN dotnet restore "TaskFlow.Api/TaskFlow.Api.csproj"

# Copiar el resto del código
COPY . .

# Compilar en Release
WORKDIR "/src/TaskFlow.Api"
RUN dotnet publish "TaskFlow.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============================================
# STAGE 2: RUNTIME
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copiar la app publicada
COPY --from=build /app/publish .

# Exponer el puerto 8080 (el default para contenedores .NET 8+)
EXPOSE 8080

# Variable de entorno para que ASP.NET Core escuche en 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TaskFlow.Api.dll"]