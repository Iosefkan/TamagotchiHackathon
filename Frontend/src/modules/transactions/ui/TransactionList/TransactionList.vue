<script lang="ts" setup>
import { toRef } from 'vue'
import type { Transactions } from '@/modules/transactions/domain/Transaction'
import TransactionCard from '@/modules/transactions/ui/TransactionCard/TransactionCard.vue'
import { useTransactionList } from '@/modules/transactions/ui/TransactionList/useCases'
import { getDateLabel } from '@/modules/transactions/ui/TransactionList/utils'

interface TransactionListProps {
  transactions: Transactions
}

const props = defineProps<TransactionListProps>()
const transactionsRef = toRef(props, 'transactions')

const { groupedTransactions } = useTransactionList(transactionsRef)
</script>

<template>
  <div class="transaction-list">
    <div v-for="group in groupedTransactions" :key="group.date" class="transaction-list__group">
      <h3 class="transaction-list__date">{{ getDateLabel(group.date) }}</h3>
      <div class="transaction-list__cards">
        <TransactionCard
          v-for="transaction in group.transactions"
          :key="transaction.uuid"
          :transaction="transaction"
        />
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.transaction-list {
  display: flex;
  flex-direction: column;
  gap: 30px;

  &__group {
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  &__date {
    font-size: 16px;
    font-weight: 700;
  }

  &__cards {
    display: flex;
    flex-direction: column;
    gap: 14px;
  }
}
</style>
