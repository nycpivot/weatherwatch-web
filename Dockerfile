FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY WeatherWatch.Web.sln .
COPY WeatherWatch.Web/*.csproj ./WeatherWatch.Web/
COPY WeatherWatch.Application/*.csproj ./WeatherWatch.Application/
RUN dotnet restore

COPY . .
RUN dotnet publish WeatherWatch.Web.sln -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app .

EXPOSE 80

ENTRYPOINT ["dotnet", "WeatherWatch.Web.dll"]
