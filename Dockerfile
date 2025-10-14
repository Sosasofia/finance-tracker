FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY *.sln .
COPY src ./src

RUN dotnet restore "FinanceTracker.sln"

COPY . .

WORKDIR "/source/src/FinanceTracker.Server"
RUN dotnet publish "FinanceTracker.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinanceTracker.Server.dll"]