<script lang="ts" setup>
import Tamagotchi from '~/modules/tamagotchi/ui/Tamagotchi/Tamagotchi.vue'
import Balance from '@/modules/transactions/ui/Balance/Balance.vue'
import UiHeader from '@/shared/ui/UiHeader/UiHeader.vue'
import { USER_MOCK } from '@/shared/mocks/user'
import BankList from '@/modules/banks/ui/BankList/BankList.vue'
import Transactions from '@/modules/transactions/ui/Transactions/Transactions.vue'

definePageMeta({
  layout: 'colorful',
})
</script>

<template>
  <div class="index-page">
    <UiHeader :user="USER_MOCK.user" />
    <div class="index-page__tamagotchi">
      <Tamagotchi class="index-page__tamagotchi-character" />
    </div>
    <div class="index-page__content">
      <Balance />
      <BankList />
      <Transactions />
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.index-page {
  &__tamagotchi {
    position: relative;
    height: 220px;

    &-character {
      width: 200px;

      position: absolute;
      top: 0;
      left: 50%;
      transform: translateX(-50%);
      cursor: pointer;
    }
  }

  &__content {
    display: flex;
    flex-direction: column;
    gap: 20px;
    margin: 0 -20px;

    isolation: isolate;

    background: variables.$white;
    border-radius: 20px;
    padding: 20px;
  }
}
</style>
