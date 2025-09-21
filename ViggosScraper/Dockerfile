FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . ./
WORKDIR "/src/ViggosScraper/"
RUN dotnet restore "ViggosScraper.csproj"
RUN dotnet build "ViggosScraper.csproj" -c Release -o /app/build
RUN dotnet publish "ViggosScraper.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS deploy
WORKDIR /app
COPY --from=build /app/publish .

# Create https directory and generate development certificate
RUN mkdir -p /https && \
    dotnet dev-certs https -ep /https/aspnetapp.pfx -p passw0rd --trust

ENTRYPOINT ["dotnet", "ViggosScraper.dll"]