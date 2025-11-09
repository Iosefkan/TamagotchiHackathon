<script lang="ts" setup>
import BankIcon from '@/modules/banks/ui/BankIcon/BankIcon.vue'
import type { Bank } from '@/modules/banks/domain'
import { bankColors } from '@/shared/lib/utils'

interface BankCardProps {
  bank: Bank
}

defineProps<BankCardProps>()
</script>

<template>
  <div class="bank-card">
    <BankIcon :color="bankColors[bank.id]" size="small" />
    <div class="bank-card__name">{{ bank.name }}</div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.bank-card {
  display: flex;
  align-items: center;
  padding: 6px;
  gap: 6px;
  border: 1px solid variables.$border-color;
  border-radius: 6px;
  height: 32px;

  &__name {
    font-size: 10px;
  }
}
</style>
