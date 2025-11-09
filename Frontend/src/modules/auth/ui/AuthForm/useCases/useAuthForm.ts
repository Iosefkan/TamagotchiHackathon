import { AuthFormType } from '~/modules/auth/ui/AuthForm/types'

export const useAuthForm = () => {
  const activeKey = ref<AuthFormType>(AuthFormType.Login);

  return {
    activeKey,
  }
}
