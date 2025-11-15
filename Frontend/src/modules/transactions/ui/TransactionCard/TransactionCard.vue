<script lang="ts" setup>
import { computed } from 'vue'
import type { Transaction } from '@/modules/transactions/domain/Transaction'
import BankIcon from '@/modules/banks/ui/BankIcon/BankIcon.vue'
import { bankColors, formatAmount } from '@/shared/lib/utils'
import { transactionOperationSymbolByType } from '@/modules/transactions/ui/TransactionCard/utils'
import { useTamagotchiSignals } from '@/modules/tamagotchi/useTamagotchiSignals'

interface TransactionCardProps {
  transaction: Transaction
}

const props = defineProps<TransactionCardProps>()

const { transactionReactions } = useTamagotchiSignals()

const reaction = computed(() => transactionReactions.value[props.transaction.uuid])
</script>

<template>
  <div class="transaction-card" :style="{ '--reaction-color': reaction ? reaction.color : 'transparent' }">
    <BankIcon class="transaction-card__bank" :color="bankColors[transaction.bankId]" />
    <div class="transaction-card__content">
      <div class="transaction-card__name">{{ transaction.name }}</div>
      <div class="transaction-card__category">{{ transaction.category }}</div>
      <div v-if="reaction" class="transaction-card__reaction" :class="`transaction-card__reaction--${reaction.tone}`">
        <span class="transaction-card__reaction-dot" />
        <span class="transaction-card__reaction-text">{{ reaction.message }}</span>
        <span v-if="reaction.isNutrition" class="transaction-card__reaction-tag">Nutrition</span>
      </div>
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
  padding: 8px 10px;
  border-radius: 14px;
  background: rgba(255, 255, 255, 0.8);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.05);
  position: relative;

  &::before {
    content: '';
    position: absolute;
    inset: 4px;
    border-radius: 12px;
    background: var(--reaction-color);
    opacity: 0.1;
    pointer-events: none;
  }

  &__bank {
    flex: 0 0 30px;
    z-index: 1;
  }

  &__content {
    display: flex;
    flex-direction: column;
    gap: 4px;
    flex: 1 1 100%;
    z-index: 1;
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

  &__reaction {
    display: flex;
    align-items: center;
    gap: 4px;
    font-size: 10px;
    text-transform: uppercase;
    letter-spacing: 0.04em;

    &-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: var(--reaction-color, variables.$gray);
    }

    &-text {
      font-weight: 600;
    }

    &-tag {
      margin-left: auto;
      padding: 2px 6px;
      border-radius: 999px;
      border: 1px solid currentColor;
    }
  }

  &__amount {
    font-size: 12px;
    z-index: 1;

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
