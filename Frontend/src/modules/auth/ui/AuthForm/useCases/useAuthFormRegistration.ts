import type { AuthFormStateRegistration } from '~/modules/auth/ui/AuthForm/types'
import { FetchError } from 'ofetch'
import { useAuthCookies } from '~/modules/auth/composables/useAuthCookies'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'

interface SignUpResponse {
  accessToken: string
  refreshToken: string
}

const DEFAULT_ERROR_MESSAGE = 'Не удалось завершить регистрацию. Попробуйте ещё раз.'

const extractErrorMessage = (error: unknown): string => {
  if (error instanceof FetchError) {
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

export const useAuthFormRegistration = () => {
  const formState = reactive<AuthFormStateRegistration>({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
  })

  const loading = ref(false)
  const errorMessage = ref<string | null>(null)

  const {
    public: { authApiBase },
  } = useRuntimeConfig()

  const router = useRouter()
  const { setTokens } = useAuthCookies()

  const disabled = computed(
    () =>
      !(
        formState.username &&
        formState.email &&
        formState.password &&
        formState.confirmPassword &&
        formState.password === formState.confirmPassword
      ),
  )

  const confirmPasswordRule = computed(() => [
    { required: true, message: 'Пожалуйста, подтвердите пароль!' },
    {
      validator: (_: unknown, value: string) => {
        if (!value || value === formState.password) {
          return Promise.resolve()
        }
        return Promise.reject(new Error('Пароли не совпадают!'))
      },
    },
  ])

  const onFinish = async () => {
    if (loading.value) return

    loading.value = true
    errorMessage.value = null

    try {
      const response = await $fetch<SignUpResponse>(`${authApiBase}/api/v1/auth/signup`, {
        method: 'POST',
        body: {
          username: formState.username,
          email: formState.email,
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
    console.error('Registration form submission failed', errorInfo)
  }

  return {
    formState,
    disabled,
    confirmPasswordRule,
    onFinish,
    onFinishFailed,
    loading,
    errorMessage,
  }
}
