<script lang="ts" setup>
import { toRef } from 'vue'
import type { Transactions } from '@/modules/transactions/domain/Transaction'
import TransactionCard from '@/modules/transactions/ui/TransactionCard/TransactionCard.vue'
import { useTransactionList } from '@/modules/transactions/ui/TransactionList/useCases'
import { getDateLabel } from '@/modules/transactions/ui/TransactionList/utils'
import { useTamagotchiSignals } from '@/modules/tamagotchi/useTamagotchiSignals'

interface TransactionListProps {
  transactions: Transactions
}

const props = defineProps<TransactionListProps>()
const transactionsRef = toRef(props, 'transactions')

const { groupedTransactions } = useTransactionList(transactionsRef)
const { reactionTrend } = useTamagotchiSignals()
</script>

<template>
  <div class="transaction-list">
    <div v-for="group in groupedTransactions" :key="group.date" class="transaction-list__group">
      <div class="transaction-list__date-row">
        <h3 class="transaction-list__date">{{ getDateLabel(group.date) }}</h3>
        <div class="transaction-list__streak">
          <span v-for="streak in reactionTrend" :key="streak.id" :style="{ backgroundColor: streak.color }" />
        </div>
      </div>
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

  &__date-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  &__streak {
    display: flex;
    gap: 4px;

    span {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      opacity: 0.8;
    }
  }

  &__cards {
    display: flex;
    flex-direction: column;
    gap: 14px;
  }
}
</style>
