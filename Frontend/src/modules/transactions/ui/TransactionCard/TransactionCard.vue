<script lang="ts" setup>
import type { Transaction } from '@/modules/transactions/domain/Transaction'
import BankIcon from '@/modules/banks/ui/BankIcon/BankIcon.vue'
import { bankColors, formatAmount } from '@/shared/lib/utils'
import { transactionOperationSymbolByType } from '@/modules/transactions/ui/TransactionCard/utils'

interface TransactionCardProps {
  transaction: Transaction
}

defineProps<TransactionCardProps>()
</script>

<template>
  <div class="transaction-card">
    <BankIcon class="transaction-card__bank" :color="bankColors[transaction.bank_id]" />
    <div class="transaction-card__content">
      <div class="transaction-card__name">{{ transaction.name }}</div>
      <div class="transaction-card__category">{{ transaction.category }}</div>
    </div>
    <div
      :class="[
        'transaction-card__amount',
        `transaction-card__amount--status-${transaction.status}`,
        `transaction-card__amount--type-${transaction.type}`,
      ]"
    >
      {{
        `${transactionOperationSymbolByType[transaction.type]}${formatAmount(transaction.amount)}`
      }}
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.transaction-card {
  display: flex;
  align-items: center;
  gap: 10px;
  width: 100%;

  &__bank {
    flex: 0 0 30px;
  }

  &__content {
    display: flex;
    flex-direction: column;
    gap: 4px;
    flex: 1 1 100%;
  }

  &__name {
    font-size: 10px;
    font-weight: 600;
  }

  &__category {
    font-size: 8px;
    font-weight: 500;
    color: variables.$gray;
  }

  &__amount {
    font-size: 12px;

    &--status {
      &-error {
        color: variables.$red;
      }
    }

    &--type {
      &-income {
        color: variables.$green;
      }
    }
  }
}
</style>
