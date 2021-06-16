#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV GOOGLE_APPLICATION_CREDENTIALS=Secure/glossy-calculus-316915-10187bb5ff27.json

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ImageTranslator.csproj", "."]
RUN dotnet restore "./ImageTranslator.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ImageTranslator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageTranslator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#ENTRYPOINT ["dotnet", "ImageTranslator.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet ImageTranslator.dll