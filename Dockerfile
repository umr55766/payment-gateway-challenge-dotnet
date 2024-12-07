FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files
COPY ["src/PaymentGateway.Api/PaymentGateway.Api.csproj", "src/PaymentGateway.Api/"]
RUN dotnet restore "src/PaymentGateway.Api/PaymentGateway.Api.csproj"

# Copy the remaining source and test files
COPY src src
RUN dotnet build "src/PaymentGateway.Api/PaymentGateway.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/PaymentGateway.Api/PaymentGateway.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:80

ENTRYPOINT ["dotnet", "PaymentGateway.Api.dll"]
