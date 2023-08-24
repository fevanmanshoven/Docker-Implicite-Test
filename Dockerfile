#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DockerImpliciteTest.csproj", "."]
RUN dotnet restore "./DockerImpliciteTest.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DockerImpliciteTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerImpliciteTest.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerImpliciteTest.dll"]