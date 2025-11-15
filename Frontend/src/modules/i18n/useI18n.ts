import { computed } from 'vue'
import { useCookie, useState } from '#imports'
import { availableLanguages, messages, type UiLanguage } from '@/modules/i18n/messages'

const STATE_KEY = 'ui-language'

export const useI18n = () => {
  const languageCookie = useCookie<UiLanguage>('ui_language', { default: () => 'en' })
  const locale = useState<UiLanguage>(STATE_KEY, () => languageCookie.value ?? 'en')

  const setLocale = (value: UiLanguage) => {
    locale.value = value
    languageCookie.value = value
    if (process.client) {
      document.documentElement.lang = value
    }
  }

  const t = (key: string): string => {
    const pack = messages[locale.value]
    return pack?.[key] ?? messages.en[key] ?? key
  }

  const currentMessages = computed(() => messages[locale.value])

  return {
    locale,
    availableLanguages,
    messages: currentMessages,
    t,
    setLocale,
  }
}


