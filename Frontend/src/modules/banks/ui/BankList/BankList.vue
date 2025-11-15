<script lang="ts" setup>
import { PlusOutlined } from '@ant-design/icons-vue'
import BankCard from '@/modules/banks/ui/BankCard/BankCard.vue'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'

const { banks } = useFinanceSummary()
</script>

<template>
  <div class="bank-list">
    <BankCard v-for="bank in banks" :key="bank.id" :bank="bank" />
    <div class="bank-list__add">
      <PlusOutlined class="bank-list__add-icon" />
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.bank-list {
  display: flex;
  gap: 6px;

  &__add {
    display: flex;
    align-items: center;
    justify-content: center;

    border-radius: 6px;
    border: 1px solid rgba(variables.$blue, 0.2);
    color: variables.$blue;
    width: 32px;
    height: 32px;

    cursor: pointer;
  }
}
</style>
