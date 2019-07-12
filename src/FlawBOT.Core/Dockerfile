FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["FlawBOT/FlawBOT.csproj", "FlawBOT/"]
COPY ["FlawBOT/NuGet.Config", "FlawBOT/"]
RUN dotnet restore "FlawBOT/FlawBOT.csproj"
COPY . .
WORKDIR "/src/FlawBOT"
RUN dotnet build "FlawBOT.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FlawBOT.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FlawBOT.dll"]