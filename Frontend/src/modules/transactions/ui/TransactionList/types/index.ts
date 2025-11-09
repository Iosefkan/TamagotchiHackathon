import type { Transactions } from '@/modules/transactions/domain/Transaction'

export interface TransactionGroup {
  date: string
  transactions: Transactions
}