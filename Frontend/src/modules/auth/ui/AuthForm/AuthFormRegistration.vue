<script lang="ts" setup>
import { LockOutlined, MailOutlined, UserOutlined } from '@ant-design/icons-vue'
import { useAuthFormRegistration } from '~/modules/auth/ui/AuthForm/useCases'

const {
  formState,
  confirmPasswordRule,
  onFinish,
  onFinishFailed,
  disabled,
  loading,
  errorMessage,
} = useAuthFormRegistration()
</script>

<template>
  <AForm
    class="auth-form-registration"
    :model="formState"
    name="register"
    layout="vertical"
    @finish="onFinish"
    @finishFailed="onFinishFailed"
  >
    <AAlert
      v-if="errorMessage"
      type="error"
      :message="errorMessage"
      show-icon
      class="auth-form-registration__error"
    />

    <div class="auth-form-registration__fields">
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
        name="email"
        :rules="[
          { required: true, message: 'Пожалуйста, введите email!' },
          { type: 'email', message: 'Введите корректный email!' },
        ]"
      >
        <AInput v-model:value="formState.email" placeholder="Email">
          <template #prefix>
            <MailOutlined class="site-form-item-icon" />
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

      <AFormItem name="confirmPassword" :rules="confirmPasswordRule">
        <AInputPassword v-model:value="formState.confirmPassword" placeholder="Подтвердите пароль">
          <template #prefix>
            <LockOutlined class="site-form-item-icon" />
          </template>
        </AInputPassword>
      </AFormItem>
    </div>

    <AFormItem class="auth-form-registration__submit">
      <AButton :disabled="disabled" :loading="loading" type="primary" html-type="submit" block>
        Зарегистрироваться
      </AButton>
    </AFormItem>
  </AForm>
</template>

<style lang="scss" scoped>
.auth-form-registration {
  display: flex;
  flex-direction: column;
  gap: 30px;

  .ant-form-item {
    margin: 0;
  }

  &__error {
    margin-bottom: 10px;
  }

  &__fields {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
}
</style>
