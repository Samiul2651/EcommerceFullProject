FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EcommerceWebApi/EcommerceWebApi.csproj", "EcommerceWebApi/"]

RUN dotnet restore "EcommerceWebApi/EcommerceWebApi.csproj"
COPY . .
WORKDIR build AS publish
RUN dotnet publish "EcommerceWebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceWebApi.dll"]