# Use a imagem oficial do .NET SDK para buildar
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copia os arquivos e restaura as dependências
COPY . . 
WORKDIR /app/MatchBox
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Use uma imagem mais leve para rodar o app
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/MatchBox/out .

# Porta padrão usada pelo Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "MatchBox.dll"]
