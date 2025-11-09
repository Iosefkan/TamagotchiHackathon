import { TransactionType } from '@/modules/transactions/domain/Transaction'

export const transactionOperationSymbolByType = {
  [TransactionType.Income]: '+',
  [TransactionType.Expense]: '-',
}
