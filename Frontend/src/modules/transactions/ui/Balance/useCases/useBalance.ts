import { BALANCE_MOCK } from '@/shared/mocks/balance'
import { formatAmount } from '@/shared/lib/utils'

export const useBalance = () => {
  const amount = BALANCE_MOCK.amount

  return {
    formattedAmount: formatAmount(amount),
  }
}
