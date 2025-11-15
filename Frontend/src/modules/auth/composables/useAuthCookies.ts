import type { CookieOptions } from 'nuxt/app'

export interface AuthTokens {
  accessToken: string
  refreshToken: string
}

const isProduction = process.env.NODE_ENV === 'production'

const baseOptions: CookieOptions = {
  sameSite: 'lax',
  secure: isProduction,
  path: '/',
}

const accessTokenOptions: CookieOptions = {
  ...baseOptions,
  maxAge: 60 * 60, // 1 hour
}

const refreshTokenOptions: CookieOptions = {
  ...baseOptions,
  maxAge: 60 * 60 * 24 * 30, // 30 days
}

export const useAuthCookies = () => {
  const accessToken = useCookie<string | null>('access_token', accessTokenOptions)
  const refreshToken = useCookie<string | null>('refresh_token', refreshTokenOptions)

  const setTokens = ({ accessToken: access, refreshToken: refresh }: AuthTokens) => {
    accessToken.value = access
    refreshToken.value = refresh
  }

  const clearTokens = () => {
    accessToken.value = null
    refreshToken.value = null
  }

  return {
    accessToken,
    refreshToken,
    setTokens,
    clearTokens,
  }
}


