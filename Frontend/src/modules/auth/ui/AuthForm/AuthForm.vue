<script lang="ts" setup>
import { useAuthForm } from '~/modules/auth/ui/AuthForm/useCases'
import AuthFormLogin from '~/modules/auth/ui/AuthForm/AuthFormLogin.vue'
import AuthFormRegistration from '~/modules/auth/ui/AuthForm/AuthFormRegistration.vue'

const { activeKey } = useAuthForm()
</script>

<template>
  <ACard class="auth-form">
    <ATabs v-model:activeKey="activeKey" centered>
      <ATabPane key="login" tab="Войти">
        <AuthFormLogin />
      </ATabPane>
      <ATabPane key="register" tab="Регистрация">
        <AuthFormRegistration />
      </ATabPane>
    </ATabs>
  </ACard>
</template>

<style lang="scss" scoped>
:deep(.ant-tabs-nav) {
  margin-bottom: 30px;
}
</style>
