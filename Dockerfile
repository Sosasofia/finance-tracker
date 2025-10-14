FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ../../*.sln .

COPY ./*.csproj ./FinanceTracker.Server/
COPY ../FinanceTracker.Application/*.csproj ./FinanceTracker.Application/
COPY ../FinanceTracker.Infrastructure/*.csproj ./FinanceTracker.Infrastructure/
COPY ../FinanceTracker.Domain/*.csproj ./FinanceTracker.Domain/

RUN dotnet restore "../../*.sln"

COPY ../../ .

WORKDIR "/src/FinanceTracker.Server"
RUN dotnet publish "FinanceTracker.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinanceTracker.Server.dll"]