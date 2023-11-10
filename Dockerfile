FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["JAPI/JAPI.csproj", "JAPI/"]
RUN dotnet restore "JAPI/JAPI.csproj"
COPY . .
WORKDIR "/src/JAPI"
RUN dotnet build "JAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JAPI.csproj" -c Release -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./JAPI"]
