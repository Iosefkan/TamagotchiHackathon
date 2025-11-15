import type { User } from '@/modules/user/domain'
import type { Banks } from '@/modules/banks/domain'
import type { Transactions } from '@/modules/transactions/domain/Transaction'
import { computed } from 'vue'
import { useRouter, useRuntimeConfig, useState } from '#imports'
import { FetchError } from 'ofetch'
import { useAuthCookies } from '@/modules/auth/composables/useAuthCookies'

interface FinanceSummaryResponse {
  user: User
  balance: number
  banks: Banks
  transactions: Transactions
}

interface FinanceSummaryState {
  data: FinanceSummaryResponse | null
  loading: boolean
  error: string | null
}

const STATE_KEY = 'finance-summary'

export const useFinanceSummary = () => {
  const state = useState<FinanceSummaryState>(STATE_KEY, () => ({
    data: null,
    loading: false,
    error: null,
  }))

  const {
    public: { apiBase },
  } = useRuntimeConfig()

  const router = useRouter()
  const { accessToken, clearTokens } = useAuthCookies()

  const redirectToAuth = async () => {
    if (router.currentRoute.value.path !== '/auth') {
      await router.push('/auth')
    }
  }

  const fetchSummary = async (): Promise<void> => {
    if (state.value.loading) return

    if (!accessToken.value) {
      state.value.data = null
      state.value.error = null
      await redirectToAuth()
      return
    }

    state.value.loading = true
    try {
      const response = await $fetch<FinanceSummaryResponse>(`${apiBase}/api/v1/finance/summary`, {
        headers: {
          Authorization: `Bearer ${accessToken.value}`,
        },
      })
      state.value.data = response
      state.value.error = null
    } catch (error) {
      let message = 'Не удалось загрузить данные'

      if (error instanceof FetchError) {
        const status = error.response?.status

        if (status === 401 || status === 403) {
          clearTokens()
          state.value.data = null
          message = 'Сессия истекла, пожалуйста, войдите снова'
          await redirectToAuth()
        } else {
          const responseMessage =
            typeof error.response?._data === 'string'
              ? error.response?._data
              : (error.response?._data as { message?: string } | undefined)?.message
          message = responseMessage ?? message
        }
      } else if (error instanceof Error) {
        message = error.message
      }

      state.value.error = message
    } finally {
      state.value.loading = false
    }
  }

  if (!state.value.data && !state.value.loading) {
    fetchSummary()
  }

  return {
    user: computed(() => state.value.data?.user ?? null),
    balance: computed(() => state.value.data?.balance ?? 0),
    banks: computed(() => state.value.data?.banks ?? []),
    transactions: computed(() => state.value.data?.transactions ?? []),
    error: computed(() => state.value.error),
    loading: computed(() => state.value.loading),
    refresh: fetchSummary,
  }
}

