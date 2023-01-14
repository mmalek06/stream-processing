FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["StreamProcessing.Server.DataIngestion/StreamProcessing.Server.DataIngestion.csproj", "StreamProcessing.Server.DataIngestion/"]
RUN dotnet restore "StreamProcessing.Server.DataIngestion/StreamProcessing.Server.DataIngestion.csproj"
COPY . .
WORKDIR "/src/StreamProcessing.Server.DataIngestion"
RUN dotnet build "StreamProcessing.Server.DataIngestion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StreamProcessing.Server.DataIngestion.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StreamProcessing.Server.DataIngestion.dll"]
