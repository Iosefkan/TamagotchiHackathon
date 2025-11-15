import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'

export const useTransactions = () => {
  const { transactions } = useFinanceSummary()

  return {
    transactions,
  }
}
