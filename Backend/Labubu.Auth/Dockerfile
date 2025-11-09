FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Labubu.Auth/Labubu.Auth.csproj", "Labubu.Auth/"]
COPY ["Labubu.Common/Labubu.Common.csproj", "Labubu.Common/"]
COPY ["Labubu.Shared/Labubu.Shared.csproj", "Labubu.Shared/"]
RUN dotnet restore "Labubu.Auth/Labubu.Auth.csproj"
COPY . .
WORKDIR "/src/Labubu.Auth"
RUN dotnet build "Labubu.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Labubu.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Labubu.Auth.dll"]


