#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
LABEL author="fha@easv.dk"
WORKDIR /app
EXPOSE 5986

# Ensure we listen on any IP Address 
ENV DOTNET_URLS=http://+:5986

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["d22rest.csproj", ""]
RUN dotnet restore "d22rest.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "d22rest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "d22rest.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "d22rest.dll"]