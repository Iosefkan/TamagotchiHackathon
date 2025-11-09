export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  ssr: false,
  devtools: { enabled: true },
  srcDir: 'src/',
  modules: [
    '@nuxt/eslint',
    '@nuxt/fonts',
    '@nuxt/icon',
    'nuxt-svgo',
    '@vite-pwa/nuxt',
  ],
  pwa: {
    manifest: {
      name: 'MultiBank Tamagotchi',
      short_name: 'MBT',
      theme_color: '#ffffff',
      background_color: '#ffffff',
      display: 'standalone',
    },
  },
  css: ['@/shared/styles/index.scss'],
  vite: {
    css: {
      preprocessorOptions: {
        scss: {
          additionalData: `
          @use "@/shared/styles/_variables.scss" as *;
          @use "@/shared/styles/_mixins.scss" as *;
        `,
        },
      },
    },
  },
  fonts: {
    families: [
      {
        name: 'Roboto',
        provider: 'google',
        weights: [400, 500, 600, 700, 800],
        styles: ['normal'],
      },
    ],
  },
  svgo: {
    autoImportPath: '@/shared/images/',
    defaultImport: 'component',
    svgoConfig: {
      multipass: true,
      plugins: [
        {
          name: 'preset-default',
          params: {
            overrides: {
              inlineStyles: {
                onlyMatchedOnce: false,
              },
              removeDoctype: false,
              removeViewBox: false,
            },
          },
        },
      ],
    },
  },
})