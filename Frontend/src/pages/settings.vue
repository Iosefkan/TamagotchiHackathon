<script lang="ts" setup>
import { computed } from 'vue'
import { useRouter } from '#imports'
import UiHeader from '@/shared/ui/UiHeader/UiHeader.vue'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'
import type { User } from '@/modules/user/domain'
import { useAuthCookies } from '@/modules/auth/composables/useAuthCookies'
import { useI18n } from '@/modules/i18n/useI18n'
import { availableLanguages } from '@/modules/i18n/messages'

definePageMeta({
  layout: 'colorful',
})

const { user } = useFinanceSummary()
const { clearTokens } = useAuthCookies()
const router = useRouter()
const { t, locale, setLocale } = useI18n()

const headerUser = computed<User>(() => ({
  id: user.value?.id ?? 'placeholder-user',
  name: user.value?.name ?? 'Labubu',
  picture: user.value?.picture ?? null,
}))

const handleLanguageChange = (language: typeof locale.value) => {
  setLocale(language)
}

const logout = async () => {
  clearTokens()
  await router.push('/auth')
}
</script>

<template>
  <div class="settings-page">
    <UiHeader :user="headerUser" />

    <div class="settings-page__heading">
      <h1>{{ t('settings.pageTitle') }}</h1>
      <p>{{ t('settings.accountDescription') }}</p>
    </div>

    <section class="settings-page__card">
      <header>
        <div>
          <p>{{ t('settings.languageTitle') }}</p>
          <small>{{ t('settings.languageDescription') }}</small>
        </div>
      </header>
      <div class="settings-page__language-grid">
        <button
          v-for="language in availableLanguages"
          :key="language.id"
          type="button"
          :class="['settings-page__language', { 'settings-page__language--active': locale === language.id }]"
          @click="handleLanguageChange(language.id)"
        >
          <span>{{ language.label }}</span>
          <strong>{{ language.nativeLabel }}</strong>
          <small v-if="locale === language.id">{{ t('settings.languageSelected') }}</small>
        </button>
      </div>
    </section>

    <section class="settings-page__card">
      <header>
        <div>
          <p>{{ t('settings.accountTitle') }}</p>
          <small>{{ t('settings.accountDescription') }}</small>
        </div>
      </header>
      <div class="settings-page__account">
        <div>
          <span>{{ headerUser.name }}</span>
          <small>ID: {{ headerUser.id }}</small>
        </div>
        <button type="button" class="settings-page__button" @click="logout">{{ t('settings.logoutButton') }}</button>
      </div>
    </section>

    <section class="settings-page__card settings-page__card--danger">
      <div>
        <p>{{ t('settings.logoutTitle') }}</p>
        <small>{{ t('settings.logoutDescription') }}</small>
      </div>
      <button type="button" class="settings-page__button settings-page__button--ghost" @click="logout">
        {{ t('settings.logoutButton') }}
      </button>
    </section>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.settings-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
  color: variables.$white;

  &__heading {
    h1 {
      margin: 0;
      font-size: 24px;
    }

    p {
      margin: 4px 0 0;
      opacity: 0.85;
    }
  }

  &__card {
    background: rgba(255, 255, 255, 0.15);
    border-radius: 20px;
    padding: 18px;
    display: flex;
    flex-direction: column;
    gap: 16px;

    header p {
      margin: 0 0 4px;
      font-size: 16px;
      font-weight: 700;
    }

    header small {
      opacity: 0.85;
    }

    &--danger {
      border: 1px solid rgba(255, 139, 139, 0.5);
      flex-direction: row;
      justify-content: space-between;
      align-items: center;
      gap: 12px;
    }
  }

  &__language-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
    gap: 12px;
  }

  &__language {
    padding: 14px;
    border-radius: 16px;
    border: 1px solid rgba(255, 255, 255, 0.3);
    background: rgba(0, 0, 0, 0.2);
    color: inherit;
    font: inherit;
    text-align: left;
    cursor: pointer;
    display: flex;
    flex-direction: column;
    gap: 4px;

    strong {
      font-size: 18px;
    }

    small {
      text-transform: uppercase;
      letter-spacing: 0.04em;
      font-size: 11px;
    }

    &--active {
      border-color: rgba(255, 255, 255, 0.8);
      box-shadow: 0 8px 26px rgba(0, 0, 0, 0.25);
      background: rgba(0, 0, 0, 0.4);
    }
  }

  &__account {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 12px;

    span {
      font-size: 18px;
      font-weight: 700;
    }

    small {
      display: block;
      opacity: 0.8;
    }
  }

  &__button {
    padding: 10px 18px;
    border-radius: 999px;
    border: none;
    font-weight: 700;
    background: variables.$white;
    color: variables.$black;
    cursor: pointer;

    &--ghost {
      background: transparent;
      border: 1px solid rgba(255, 255, 255, 0.7);
      color: variables.$white;
    }
  }
}

@media (max-width: 768px) {
  .settings-page__card--danger {
    flex-direction: column;
    align-items: flex-start;
  }

  .settings-page__account {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>