import { formatAmount } from '@/shared/lib/utils'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'

export const useBalance = () => {
  const { balance } = useFinanceSummary()

  return {
    formattedAmount: computed(() => formatAmount(balance.value)),
  }
}
