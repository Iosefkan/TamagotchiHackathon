import { ref, watch, toValue, type MaybeRef } from 'vue'
import { groupTransactions, sortGroups, sortTransactions } from '../utils'
import type { Transactions } from '@/modules/transactions/domain/Transaction'
import type { TransactionGroup } from '@/modules/transactions/ui/TransactionList/types'

export const useTransactionList = (transactionsSource: MaybeRef<Transactions>) => {
  const groupedTransactions = ref<TransactionGroup[]>([])

  const groupAndSort = () => {
    const transactions = toValue(transactionsSource) ?? []
    if (!transactions.length) {
      groupedTransactions.value = []
      return
    }

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
  }

  groupAndSort()

  watch(
    () => toValue(transactionsSource),
    () => {
      groupAndSort()
    },
    { deep: true },
  )

  return { groupedTransactions }
}
