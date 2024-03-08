FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY WeatherWatch.Web.sln .
COPY WeatherWatch.Web/WeatherWatch.Web.csproj .
COPY WeatherWatch.Application/WeatherWatch.Application.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish WeatherWatch.Web.sln -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "WeatherWatch.Web.dll"]