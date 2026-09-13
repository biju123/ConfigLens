FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ConfigLens.slnx ./
COPY src/ConfigLens.Domain/ConfigLens.Domain.csproj src/ConfigLens.Domain/
COPY src/ConfigLens.SampleData/ConfigLens.SampleData.csproj src/ConfigLens.SampleData/
COPY src/ConfigLens.Infrastructure/ConfigLens.Infrastructure.csproj src/ConfigLens.Infrastructure/
COPY src/ConfigLens.Application/ConfigLens.Application.csproj src/ConfigLens.Application/
COPY src/ConfigLens.Api/ConfigLens.Api.csproj src/ConfigLens.Api/
RUN dotnet restore src/ConfigLens.Api/ConfigLens.Api.csproj

COPY src/ ./src/
RUN dotnet publish src/ConfigLens.Api/ConfigLens.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# The runtime image has neither curl nor wget, so the healthcheck opens a
# raw TCP connection to confirm Kestrel is accepting requests on :8080.
HEALTHCHECK --interval=10s --timeout=3s --start-period=10s --retries=5 \
    CMD bash -c 'exec 3<>/dev/tcp/127.0.0.1/8080' || exit 1

ENTRYPOINT ["dotnet", "ConfigLens.Api.dll"]
