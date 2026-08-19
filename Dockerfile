# ===== 阶段 1: 构建 Vue 前端 =====
FROM node:22-alpine AS vue-builder
WORKDIR /app/vue

COPY GlueWeb/package*.json ./
RUN npm ci

COPY GlueWeb/ ./
RUN npm run build

# ===== 阶段 2: 构建 .NET Core API =====
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api-build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY GlueBackend/Glue.API/Glue.API.csproj ./
RUN dotnet restore "./Glue.API.csproj"

COPY GlueBackend/Glue.API/ ./
RUN dotnet build "./Glue.API.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "./Glue.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== 阶段 3: 组装最终镜像 =====
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=api-build /app/publish .
COPY --from=vue-builder /app/vue/dist ./wwwroot

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Glue.API.dll"]