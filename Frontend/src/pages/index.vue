<script lang="ts" setup>
import { computed } from 'vue'
import { useRouter } from '#imports'
import Tamagotchi from '~/modules/tamagotchi/ui/Tamagotchi/Tamagotchi.vue'
import Balance from '@/modules/transactions/ui/Balance/Balance.vue'
import UiHeader from '@/shared/ui/UiHeader/UiHeader.vue'
import BankList from '@/modules/banks/ui/BankList/BankList.vue'
import Transactions from '@/modules/transactions/ui/Transactions/Transactions.vue'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'
import type { User } from '@/modules/user/domain'
import { useTamagotchiSignals } from '@/modules/tamagotchi/useTamagotchiSignals'
import { useI18n } from '@/modules/i18n/useI18n'

definePageMeta({
  layout: 'colorful',
})

const { user } = useFinanceSummary()
const { tamagotchiStage, seasonalMutation } = useTamagotchiSignals()
const router = useRouter()
const { t } = useI18n()

const headerUser = computed<User>(() => ({
  id: user.value?.id ?? 'placeholder-user',
  name: user.value?.name ?? 'Labubu',
  picture: user.value?.picture ?? null,
}))

const goToTamagotchi = async () => {
  await router.push('/tamagotchi')
}

const goToStatistics = async () => {
  await router.push('/statistics')
}
</script>

<template>
  <div class="index-page">
    <UiHeader :user="headerUser" />
    <section class="index-page__layout">
      <article class="index-page__panel index-page__panel--hero">
        <div class="index-page__creature">
          <Tamagotchi />
          <div class="index-page__stage">
            <div>
              <p>{{ tamagotchiStage.name }}</p>
              <small>{{ tamagotchiStage.descriptor }}</small>
            </div>
            <div class="index-page__mutation">
              <span>{{ seasonalMutation.season }}</span>
              <strong>{{ seasonalMutation.ready ? 'Ready' : seasonalMutation.requirement }}</strong>
            </div>
          </div>
        </div>
        <div class="index-page__hero-actions">
          <button type="button" class="index-page__button" @click="goToTamagotchi">
            {{ t('index.openCreature') }}
          </button>
          <button type="button" class="index-page__button index-page__button--ghost" @click="goToStatistics">
            {{ t('index.nutritionStats') }}
          </button>
        </div>
      </article>
      <article class="index-page__panel index-page__panel--stack">
        <Balance />
        <BankList />
        <Transactions />
      </article>
    </section>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.index-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
  color: variables.$white;

  &__layout {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
    gap: 18px;
  }

  &__panel {
    border-radius: 20px;
    padding: 18px;
    background: rgba(255, 255, 255, 0.08);
    backdrop-filter: blur(8px);

    &--stack {
      display: flex;
      flex-direction: column;
      gap: 16px;
      background: variables.$white;
      color: variables.$black;
    }
  }

  &__creature {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 16px;
  }

  &__stage {
    width: 100%;
    display: flex;
    justify-content: space-between;
    gap: 12px;
    padding: 12px;
    border-radius: 14px;
    background: rgba(0, 0, 0, 0.25);

    p {
      margin: 0;
      font-weight: 700;
    }

    small {
      font-size: 12px;
      opacity: 0.9;
    }
  }

  &__mutation {
    text-align: right;
    display: flex;
    flex-direction: column;
    gap: 4px;

    span {
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 0.04em;
    }

    strong {
      font-size: 12px;
      letter-spacing: 0.04em;
    }
  }

  &__hero-actions {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
    margin-top: 10px;
  }

  &__button {
    flex: 1 1 140px;
    padding: 10px 14px;
    border-radius: 999px;
    border: none;
    background: variables.$white;
    color: variables.$black;
    font-weight: 700;
    cursor: pointer;
    transition: opacity 0.2s ease;

    &:hover {
      opacity: 0.85;
    }

    &--ghost {
      background: transparent;
      border: 1px solid rgba(255, 255, 255, 0.6);
      color: variables.$white;
    }
  }
}

@media (max-width: 768px) {
  .index-page {
    &__panel {
      padding: 14px;
    }

    &__stage {
      flex-direction: column;
    }

    &__hero-actions {
      flex-direction: column;
    }
  }
}
</style>
