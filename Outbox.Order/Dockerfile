FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Outbox.Order/Outbox.Order.csproj", "Outbox.Order/"]
COPY ["Outbox.Common/Outbox.Common.csproj", "Outbox.Common/"]
RUN dotnet restore "Outbox.Order/Outbox.Order.csproj"
COPY . .
WORKDIR "/src/Outbox.Order"
RUN dotnet build "Outbox.Order.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Outbox.Order.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Outbox.Order.dll"]
