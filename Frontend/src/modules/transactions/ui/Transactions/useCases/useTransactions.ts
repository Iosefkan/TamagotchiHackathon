import type { Transactions } from '@/modules/transactions/domain/Transaction'
import { TRANSACTIONS_MOCK } from '@/shared/mocks/transactions'

export const useTransactions = () => {
  const transactions = ref<Transactions>([])

  transactions.value = TRANSACTIONS_MOCK.transactions

  return {
    transactions,
  }
}
