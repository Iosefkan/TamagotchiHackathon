import type { AuthFormStateRegistration } from '~/modules/auth/ui/AuthForm/types'

export const useAuthFormRegistration = () => {
  const formState = reactive<AuthFormStateRegistration>({
    username: '',
    password: '',
    confirmPassword: '',
  })

  const disabled = computed(() => {
    return !(formState.username && formState.password && formState.confirmPassword)
  })

  const confirmPasswordRule = computed(() => [
    { required: true, message: 'Пожалуйста, подтвердите пароль!' },
    {
      validator: (_: any, value: string) => {
        if (!value || value === formState.password) {
          return Promise.resolve()
        }
        return Promise.reject('Пароли не совпадают!')
      },
    },
  ])

  const onFinish = (values: any) => {
    console.log('Success:', values)
  }

  const onFinishFailed = (errorInfo: any) => {
    console.log('Failed:', errorInfo)
  }


  return {
    formState,
    disabled,
    confirmPasswordRule,
    onFinish,
    onFinishFailed,
  }
}
