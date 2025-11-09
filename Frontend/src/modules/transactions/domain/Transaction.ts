export enum TransactionType {
  Expense = 'expense',
  Income = 'income',
}

export enum TransactionStatus {
  Pending = 'pending',
  Success = 'success',
  Error = 'error',
}

export interface Transaction {
  uuid: string
  bank_id: string
  name: string
  category: string
  type: TransactionType
  amount: number
  status: TransactionStatus
  date: string
}

export type Transactions = Transaction[]
