<script lang="ts" setup>
import { LockOutlined, UserOutlined } from '@ant-design/icons-vue'
import { useAuthFormLogin } from '~/modules/auth/ui/AuthForm/useCases/useAuthFormLogin'

const { formState, onFinish, onFinishFailed, disabled } = useAuthFormLogin()
</script>

<template>
  <AForm
    class="auth-form-login"
    :model="formState"
    name="login"
    layout="vertical"
    @finish="onFinish"
    @finishFailed="onFinishFailed"
  >
    <div class="auth-form-login__fields">
      <AFormItem
        name="username"
        :rules="[{ required: true, message: 'Пожалуйста, введите имя пользователя!' }]"
      >
        <AInput v-model:value="formState.username" placeholder="Имя пользователя">
          <template #prefix>
            <UserOutlined class="site-form-item-icon" />
          </template>
        </AInput>
      </AFormItem>

      <AFormItem
        name="password"
        :rules="[{ required: true, message: 'Пожалуйста, введите пароль!' }]"
      >
        <AInputPassword v-model:value="formState.password" placeholder="Пароль">
          <template #prefix>
            <LockOutlined class="site-form-item-icon" />
          </template>
        </AInputPassword>
      </AFormItem>
    </div>

    <AFormItem class="auth-form-login__submit">
      <AButton :disabled="disabled" type="primary" html-type="submit" block>Войти</AButton>
    </AFormItem>
  </AForm>
</template>

<style lang="scss" scoped>
.auth-form-login {
  display: flex;
  flex-direction: column;
  gap: 30px;

  .ant-form-item {
    margin: 0;
  }

  &__fields {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
}
</style>
