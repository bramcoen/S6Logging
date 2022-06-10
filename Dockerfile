#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . ./build
RUN dotnet restore "build/S6Logging/S6Logging.csproj"
WORKDIR "/src/build/S6Logging"
#COPY . ./
RUN dotnet build "S6Logging.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "S6Logging.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "S6Logging.dll"]