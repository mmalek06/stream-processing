FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["StreamProcessing.DataSender/StreamProcessing.DataSender.csproj", "StreamProcessing.DataSender/"]
RUN dotnet restore "StreamProcessing.DataSender/StreamProcessing.DataSender.csproj"
COPY . .
WORKDIR "/src/StreamProcessing.DataSender"
RUN dotnet build "StreamProcessing.DataSender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StreamProcessing.DataSender.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StreamProcessing.DataSender.dll"]
