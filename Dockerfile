# ── 階段 1:base ── 執行環境(只有 ASP.NET Runtime,沒有 SDK)─────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
# 容器內讓 Kestrel 聽 8080(HTTP)。EXPOSE 只是標示,要用 docker run -p 映射才連得到
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ── 階段 2:build ── 用 SDK 還原套件並編譯 ───────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 先只複製三個 csproj 再 restore:原始碼改了但套件沒變時,這一層可以沿用快取
COPY ["APILayer/APILayer.csproj", "APILayer/"]
COPY ["BusinessLogicLayer/BusinessLogicLayer.csproj", "BusinessLogicLayer/"]
COPY ["DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]
RUN dotnet restore "APILayer/APILayer.csproj"

# 再複製全部原始碼並編譯
COPY . .
WORKDIR "/src/APILayer"
RUN dotnet build "APILayer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ── 階段 3:publish ── 產生可直接執行的發佈檔 ────────────────────────
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "APILayer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ── 階段 4:final ── 成品 = Runtime + 發佈檔(不含 SDK、原始碼)──────────
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APILayer.dll"]
