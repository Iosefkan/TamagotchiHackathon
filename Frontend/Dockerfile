FROM node:20-alpine AS base
ENV NODE_ENV=production
WORKDIR /app

FROM base AS deps
RUN corepack enable
COPY package.json yarn.lock ./
RUN yarn install --frozen-lockfile

FROM deps AS build
COPY . .
RUN yarn build

FROM base AS runner
WORKDIR /app
ENV NITRO_PORT=3000
ENV NUXT_PORT=3000
ENV NUXT_HOST=0.0.0.0
EXPOSE 3000
COPY --from=build /app/.output ./.output
CMD ["node", ".output/server/index.mjs"]

