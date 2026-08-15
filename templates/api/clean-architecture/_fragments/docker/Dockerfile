FROM mcr.microsoft.com/dotnet/sdk:{{DotnetVersion}} AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish src/{{ProjectName}}.Api -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:{{DotnetVersion}}
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "{{ProjectName}}.Api.dll"]
