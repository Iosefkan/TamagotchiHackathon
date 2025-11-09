FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Labubu.Transactions/Labubu.Transactions.csproj", "Labubu.Transactions/"]
COPY ["Labubu.Shared/Labubu.Shared.csproj", "Labubu.Shared/"]
RUN dotnet restore "Labubu.Transactions/Labubu.Transactions.csproj"
COPY . .
WORKDIR "/src/Labubu.Transactions"
RUN dotnet build "Labubu.Transactions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Labubu.Transactions.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Labubu.Transactions.dll"]


