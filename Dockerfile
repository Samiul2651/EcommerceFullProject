FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EcommerceWebApi/EcommerceWebApi.csproj", "EcommerceWebApi/"]
COPY ["Business/Business.csproj", "Business/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
RUN dotnet restore "EcommerceWebApi/EcommerceWebApi.csproj"

COPY . .
WORKDIR "/src/EcommerceWebApi"
RUN dotnet publish "EcommerceWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EcommerceWebApi.dll"]
