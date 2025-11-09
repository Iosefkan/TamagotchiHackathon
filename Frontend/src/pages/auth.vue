<script lang="ts" setup>
import LogoIcon from '@/shared/images/logo.svg'
import TamagotchiImage from '~/shared/images/tamagotchi/default.webp'
import AuthForm from '~/modules/auth/ui/AuthForm/AuthForm.vue'

definePageMeta({
  layout: 'colorful',
})
</script>

<template>
  <div class="auth-page">
    <div class="auth-page__logo">
      <LogoIcon  />
    </div>
    <div class="auth-page__tamagotchi">
      <img :src="TamagotchiImage" alt="tamagotchi" />
    </div>
    <AuthForm />
  </div>
</template>

<style lang="scss" scoped>
.auth-page {
  display: flex;
  align-items: center;
  flex-direction: column;
  position: relative;

  &__logo {
    width: 400px;
    margin: 50px 0 230px 0;

    svg {
      width: 100%;
      height: 100%;
    }
  }

  &__tamagotchi {
    position: absolute;
    top: 100px;

    width: 300px;

    img {
      height: 100%;
      width: 100%;
      object-fit: contain;
    }
  }

  .auth-form {
    width: 400px;
  }
}
</style>
