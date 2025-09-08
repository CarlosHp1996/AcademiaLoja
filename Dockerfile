FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos .csproj primeiro para restaurar as dependências
COPY ["AcademiaLoja/AcademiaLoja.Web.csproj", "AcademiaLoja/"]
COPY ["AcademiaLoja.Application/AcademiaLoja.Application.csproj", "AcademiaLoja.Application/"]
COPY ["AcademiaLoja.Domain/AcademiaLoja.Domain.csproj", "AcademiaLoja.Domain/"]
COPY ["AcademiaLojaInfra/AcademiaLoja.Infra.csproj", "AcademiaLojaInfra/"]
COPY ["CrudGenerator/CrudGenerator.csproj", "CrudGenerator/"]

# Restaura as dependências
RUN dotnet restore "AcademiaLoja/AcademiaLoja.Web.csproj"

# Copia todo o código fonte
COPY . .

# Compila o projeto
WORKDIR "/src/AcademiaLoja"
RUN dotnet build "AcademiaLoja.Web.csproj" -c Release -o /app/build

# Publica o projeto
FROM build AS publish
RUN dotnet publish "AcademiaLoja.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Cria diretório para imagens (se necessário)
RUN mkdir -p /app/ImagensBackend

ENTRYPOINT ["dotnet", "AcademiaLoja.Web.dll"]