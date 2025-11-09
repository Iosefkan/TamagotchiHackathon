import { groupTransactions, sortGroups, sortTransactions } from '../utils'
import type { Transactions } from '@/modules/transactions/domain/Transaction'
import type { TransactionGroup } from '@/modules/transactions/ui/TransactionList/types'

export const useTransactionList = (transactions: Transactions) => {
  const groupedTransactions = ref<TransactionGroup[]>([])

  const groupAndSort = () => {
    if (!transactions || !transactions.length) return

    const groups = groupTransactions(transactions)

    Object.keys(groups).forEach((key) => {
      if (!groups[key]) return
      groups[key] = sortTransactions(groups[key])
    })

    const sortedKeys = sortGroups(groups)

    groupedTransactions.value = sortedKeys.map((key) => ({
      date: key,
      transactions: groups[key],
    })) as TransactionGroup[]

    console.log(groupedTransactions)
  }

  groupAndSort()

  watch(transactions, () => {
    groupAndSort()
  }, { deep: true })

  return { groupedTransactions }
}
