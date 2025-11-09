import type { Transactions } from '@/modules/transactions/domain/Transaction'

const TODAY = 'Сегодня'
const YESTERDAY = 'Вчера'

export const formatTransactionDate = (date: Date): string => date.toISOString().split('T')[0] || ''

export const getDateLabel = (date: string): string => {
  const ISODate = new Date(date)

  const today = new Date()
  const yesterday = new Date()
  yesterday.setDate(today.getDate() - 1)

  const txnDateStr = formatTransactionDate(ISODate)

  if (txnDateStr === formatTransactionDate(today)) return TODAY
  if (txnDateStr === formatTransactionDate(yesterday)) return YESTERDAY

  return ISODate.toLocaleDateString('ru-RU', { day: '2-digit', month: 'long', year: 'numeric' })
}

export const groupTransactions = (transactions: Transactions): Record<string, Transactions> => {
  return transactions.reduce(
    (acc, txn) => {
      const dateKey = formatTransactionDate(new Date(txn.date))
      if (!acc[dateKey]) acc[dateKey] = []
      acc[dateKey].push(txn)
      return acc
    },
    {} as Record<string, Transactions>,
  )
}

export const sortTransactions = (transactions: Transactions): Transactions =>
  [...transactions].sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())

export const sortGroups = (groups: Record<string, Transactions>): string[] => {
  const order: Record<string, number> = { [TODAY]: 0, [YESTERDAY]: 1 }

  return Object.keys(groups).sort((a, b) => {
    // Сегодня и Вчера всегда сверху
    if (order[a] !== undefined) return order[a] - (order[b] ?? 2)
    if (order[b] !== undefined) return (order[a] ?? 2) - order[b]

    // для остальных ISO-ключей сортируем по дате
    return new Date(b).getTime() - new Date(a).getTime()
  })
}