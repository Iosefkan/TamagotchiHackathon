<script lang="ts" setup>
import { useBalance } from '@/modules/transactions/ui/Balance/useCases'

const { formattedAmount } = useBalance()
</script>

<template>
  <div class="balance">
    <div class="balance__sum">
      {{ formattedAmount }}
    </div>
    <div class="balance__hint">
      сумма на всех счетах
    </div>
  </div>
</template>

<style lang="scss" scoped>
.balance {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;

  &__sum {
    font-size: 24px;
    font-weight: 700;
  }

  &__hint {
    color: $gray;
    font-size: 8px;
  }
}
</style>