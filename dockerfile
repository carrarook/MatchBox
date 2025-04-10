# Build com .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /app

COPY . .
WORKDIR /app/MatchBox
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Runtime com .NET 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/MatchBox/out .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "MatchBox.dll"]
