# ---------- 1. Build ----------
FROM oven/bun:1 AS build
WORKDIR /app

# Nejdřív jen package.json + lock → Docker si vrstvu s balíčky zacachuje
# a při změně kódu je nebude stahovat znovu
COPY package.json bun.lock ./
RUN bun install --frozen-lockfile

COPY . .
RUN bun run build

# ---------- 2. Serve ----------
FROM nginx:1.27-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80