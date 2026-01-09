# Usar SDK para desenvolvimento (inclui ferramentas EF)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development

WORKDIR /src

# Copiar arquivos de projeto
COPY ["DeviceWebApi/DeviceWebApi.csproj", "DeviceWebApi/"]
COPY ["DeviceWebApi.Data/DeviceWebApi.Data.csproj", "DeviceWebApi.Data/"]
COPY ["DeviceWebApi.Core/DeviceWebApi.Core.csproj", "DeviceWebApi.Core/"]

# Restaurar pacotes
RUN dotnet restore "DeviceWebApi/DeviceWebApi.csproj"

# Copiar todo o código
COPY . .

# Instalar dotnet-ef globalmente
RUN dotnet tool install --global dotnet-ef --version 9.0.0

# Adicionar dotnet-ef ao PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Porta padrão
EXPOSE 8080

# Comando para desenvolvimento (será sobrescrito pelo docker-compose)
CMD ["dotnet", "run", "--project", "DeviceWebApi"]