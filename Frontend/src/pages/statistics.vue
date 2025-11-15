<script lang="ts" setup>
import { computed } from 'vue'
import UiHeader from '@/shared/ui/UiHeader/UiHeader.vue'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'
import type { User } from '@/modules/user/domain'
import { useTamagotchiSignals } from '@/modules/tamagotchi/useTamagotchiSignals'
import { useI18n } from '@/modules/i18n/useI18n'

definePageMeta({
  layout: 'colorful',
})

const { user } = useFinanceSummary()
const { nutritionPrompts, progressBars } = useTamagotchiSignals()
const { t } = useI18n()

const headerUser = computed<User>(() => ({
  id: user.value?.id ?? 'placeholder-user',
  name: user.value?.name ?? 'Labubu',
  picture: user.value?.picture ?? null,
}))
</script>

<template>
  <div class="statistics-page">
    <UiHeader :user="headerUser" />

    <section class="statistics-page__nutrition">
      <header>
        <h3>{{ t('statistics.nutrition.title') }}</h3>
        <p>{{ t('statistics.nutrition.subtitle') }}</p>
      </header>
      <div class="statistics-page__nutrition-grid">
        <article v-for="prompt in nutritionPrompts" :key="prompt.goal" :class="['statistics-page__card', { 'statistics-page__card--positive': prompt.onTrack }]">
          <h4>{{ prompt.goal }}</h4>
          <p>{{ prompt.status }}</p>
          <strong>{{ prompt.action }}</strong>
          <small>{{ prompt.highlight }}</small>
        </article>
      </div>
    </section>

    <section class="statistics-page__overview">
      <header>
        <h3>{{ t('statistics.overview.title') }}</h3>
        <p>{{ t('statistics.overview.subtitle') }}</p>
      </header>
      <div class="statistics-page__overview-grid">
        <article v-for="bar in progressBars" :key="bar.id">
          <span>{{ bar.label }}</span>
          <strong>{{ bar.value }}%</strong>
          <small>{{ bar.description }}</small>
        </article>
      </div>
    </section>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.statistics-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
  color: variables.$white;

  section {
    background: rgba(255, 255, 255, 0.15);
    border-radius: 20px;
    padding: 18px;

    header {
      margin-bottom: 12px;

      h3 {
        margin: 0;
        text-transform: uppercase;
        letter-spacing: 0.04em;
      }

      p {
        margin: 4px 0 0;
        font-size: 13px;
      }
    }
  }

  &__nutrition-grid,
  &__overview-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
    gap: 12px;
  }

  &__card {
    background: rgba(0, 0, 0, 0.2);
    border-radius: 16px;
    padding: 12px;

    &--positive {
      border: 1px solid rgba(123, 255, 196, 0.4);
    }

    h4 {
      margin: 0 0 4px;
    }

    strong {
      display: block;
      margin-top: 6px;
    }
  }

  &__overview-grid article {
    background: rgba(0, 0, 0, 0.2);
    border-radius: 16px;
    padding: 12px;
    display: flex;
    flex-direction: column;
    gap: 4px;

    span {
      text-transform: uppercase;
      font-size: 12px;
      letter-spacing: 0.04em;
    }

    strong {
      font-size: 22px;
    }
  }
}
</style>