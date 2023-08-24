# To learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ARG TARGETARCH
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DockerImpliciteTest.csproj", "."]
RUN dotnet restore "./DockerImpliciteTest.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DockerImpliciteTest.csproj" -c Release -o /app/build

# copy and publish app and libraries
FROM build AS publish
RUN dotnet publish -a $TARGETARCH --self-contained false --no-restore "DockerImpliciteTest.csproj" -c Release -o /app/publish /p:UseAppHost=false

# To enable globalization:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/enable-globalization.md
# final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerImpliciteTest.dll"]