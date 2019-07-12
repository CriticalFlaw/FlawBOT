FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["FlawBOT.Core/FlawBOT.Core.csproj", "FlawBOT.Core/"]
COPY ["FlawBOT.Core/NuGet.Config", "FlawBOT.Core/"]
COPY ["FlawBOT.Framework/FlawBOT.Framework.csproj", "FlawBOT.Framework/"]
RUN dotnet restore "FlawBOT.Core/FlawBOT.Core.csproj"
COPY . .
WORKDIR "/src/FlawBOT.Core"
RUN dotnet build "FlawBOT.Core.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FlawBOT.Core.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FlawBOT.Core.dll"]