import type { AuthFormStateLogin } from '~/modules/auth/ui/AuthForm/types'

export const useAuthFormLogin = () => {
  const formState = reactive<AuthFormStateLogin>({
    username: '',
    password: '',
  })

  const onFinish = (values: any) => {
    console.log('Success:', values)
  }

  const onFinishFailed = (errorInfo: any) => {
    console.log('Failed:', errorInfo)
  }
  const disabled = computed(() => {
    return !(formState.username && formState.password)
  })

  return {
    formState,
    onFinish,
    onFinishFailed,
    disabled,
  }
}
