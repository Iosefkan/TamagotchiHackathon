import type { AuthFormStateLogin } from '~/modules/auth/ui/AuthForm/types'
import { FetchError } from 'ofetch'
import { useAuthCookies } from '~/modules/auth/composables/useAuthCookies'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'

interface SignInResponse {
  accessToken: string
  refreshToken: string
}

const DEFAULT_ERROR_MESSAGE = 'Не удалось выполнить вход. Попробуйте ещё раз.'

const extractErrorMessage = (error: unknown): string => {
  if (error instanceof FetchError) {
    if (error.response?.status === 401) {
      return 'Неверное имя пользователя или пароль'
    }

    const responseMessage =
      typeof error.response?._data === 'string'
        ? error.response?._data
        : (error.response?._data as { message?: string } | undefined)?.message

    return responseMessage ?? DEFAULT_ERROR_MESSAGE
  }

  if (error instanceof Error) {
    return error.message
  }

  return DEFAULT_ERROR_MESSAGE
}

export const useAuthFormLogin = () => {
  const formState = reactive<AuthFormStateLogin>({
    username: '',
    password: '',
  })

  const loading = ref(false)
  const errorMessage = ref<string | null>(null)

  const {
    public: { authApiBase },
  } = useRuntimeConfig()

  const router = useRouter()
  const { setTokens } = useAuthCookies()

  const onFinish = async () => {
    if (loading.value) return

    loading.value = true
    errorMessage.value = null

    try {
      const response = await $fetch<SignInResponse>(`${authApiBase}/api/v1/auth/signin`, {
        method: 'POST',
        body: {
          username: formState.username,
          password: formState.password,
        },
      })

      setTokens(response)

      const { refresh } = useFinanceSummary()
      await refresh()
      await router.push('/')
    } catch (error) {
      errorMessage.value = extractErrorMessage(error)
    } finally {
      loading.value = false
    }
  }

  const onFinishFailed = (errorInfo: unknown) => {
    console.error('Login form submission failed', errorInfo)
  }

  const disabled = computed(() => !(formState.username && formState.password))

  return {
    formState,
    onFinish,
    onFinishFailed,
    disabled,
    loading,
    errorMessage,
  }
}
