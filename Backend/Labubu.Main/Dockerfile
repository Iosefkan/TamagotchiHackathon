FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Labubu.Main/Labubu.Main.csproj", "Labubu.Main/"]
COPY ["Labubu.Common/Labubu.Common.csproj", "Labubu.Common/"]
COPY ["Labubu.Shared/Labubu.Shared.csproj", "Labubu.Shared/"]
RUN dotnet restore "Labubu.Main/Labubu.Main.csproj"
COPY . .
WORKDIR "/src/Labubu.Main"
RUN dotnet build "Labubu.Main.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Labubu.Main.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Labubu.Main.dll"]


