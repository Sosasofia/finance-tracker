FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY src/FinanceTracker.Application ./src/FinanceTracker.Application
COPY src/FinanceTracker.Domain ./src/FinanceTracker.Domain
COPY src/FinanceTracker.Infrastructure ./src/FinanceTracker.Infrastructure
COPY src/FinanceTracker.Server ./src/FinanceTracker.Server

RUN dotnet restore ./src/FinanceTracker.Server/FinanceTracker.Server.csproj

RUN dotnet publish ./src/FinanceTracker.Server/FinanceTracker.Server.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FinanceTracker.Server.dll"]