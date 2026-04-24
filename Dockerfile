# Immagine di base per il build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia i file di progetto e ripristina le dipendenze
COPY ["EntangoApi/EntangoApi.csproj", "EntangoApi/"]
RUN dotnet restore "EntangoApi/EntangoApi.csproj"

# Copia il resto del codice e compila
COPY . .
WORKDIR "/src/EntangoApi"
RUN dotnet build "EntangoApi.csproj" -c Release -o /app/build

# Pubblica l'applicazione
FROM build AS publish
RUN dotnet publish "EntangoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Immagine finale
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copia l'applicazione pubblicata
COPY --from=publish /app/publish .

# Imposta l'entrypoint
ENTRYPOINT ["dotnet", "EntangoApi.dll"]