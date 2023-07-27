FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . ./
WORKDIR "/src/ViggosScraper/"
RUN dotnet restore "ViggosScraper.csproj"
RUN dotnet build "ViggosScraper.csproj" -c Release -o /app/build
RUN dotnet publish "ViggosScraper.csproj" -c Release -o /app/publish

#generate the cert, define the path to store it and password to use
RUN dotnet dev-certs https -ep /https/aspnetapp.pfx -p passw0rd

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS deploy
WORKDIR /ticketbot
COPY --from=build /https/* /https/
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ViggosScraper.dll"]