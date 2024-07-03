FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV Logging__EventLog__LogLevel__Default None
ENV ASPNETCORE_ENVIRONMENT Docker

ENTRYPOINT ["dotnet", "EventPlanner.dll"]