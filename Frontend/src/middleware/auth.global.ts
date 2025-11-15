export default defineNuxtRouteMiddleware((to) => {
  const accessToken = useCookie<string | null>('access_token')
  const isAuthRoute = to.path === '/auth'

  if (!accessToken.value && !isAuthRoute) {
    return navigateTo('/auth')
  }

  if (accessToken.value && isAuthRoute) {
    return navigateTo('/')
  }
})