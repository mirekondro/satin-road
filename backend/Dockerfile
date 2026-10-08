# ---------- 1. Build: .NET SDK zkompiluje a publikuje API ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Nejdřív jen .csproj soubory → restore balíčků se zacachuje
# a při změně kódu se NuGet nestahuje znovu (stejný trik jako bun install)
COPY SatinRoad.Api/SatinRoad.Api.csproj SatinRoad.Api/
COPY SatinRoad.Core/SatinRoad.Core.csproj SatinRoad.Core/
RUN dotnet restore SatinRoad.Api/SatinRoad.Api.csproj

# Pak zbytek kódu a publish (Release = optimalizovaný build)
COPY . .
RUN dotnet publish SatinRoad.Api/SatinRoad.Api.csproj -c Release -o /app --no-restore

# ---------- 2. Runtime: jen ASP.NET runtime + hotové DLL ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# .NET 10 kontejner poslouchá defaultně na 8080 (na to míří nginx)
EXPOSE 8080
ENTRYPOINT ["dotnet", "SatinRoad.Api.dll"]