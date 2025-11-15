<script lang="ts" setup>
import { computed, ref, watchEffect } from 'vue'
import UiHeader from '@/shared/ui/UiHeader/UiHeader.vue'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'
import type { User } from '@/modules/user/domain'
import { useTamagotchiSignals } from '@/modules/tamagotchi/useTamagotchiSignals'
import { useI18n } from '@/modules/i18n/useI18n'

definePageMeta({
  layout: 'colorful',
})

const { user } = useFinanceSummary()
const { achievements } = useTamagotchiSignals()
const { t } = useI18n()

const headerUser = computed<User>(() => ({
  id: user.value?.id ?? 'placeholder-user',
  name: user.value?.name ?? 'Labubu',
  picture: user.value?.picture ?? null,
}))

const rarityClass = (rarity: string) => `achievements-page__card--${rarity}`

const selectedAchievementId = ref<string | null>(null)

watchEffect(() => {
  if (!achievements.value.length || selectedAchievementId.value) return
  selectedAchievementId.value = achievements.value[0].id
})

const activeAchievement = computed(() =>
  achievements.value.find((achievement) => achievement.id === selectedAchievementId.value) ??
  achievements.value[0] ??
  null,
)

const handleSelectAchievement = (achievementId: string) => {
  selectedAchievementId.value = achievementId
}
</script>

<template>
  <div class="achievements-page">
    <UiHeader :user="headerUser" />

    <section class="achievements-page__grid">
      <button
        v-for="achievement in achievements"
        :key="achievement.id"
        type="button"
        :class="[
          'achievements-page__card',
          rarityClass(achievement.rarity),
          { 'achievements-page__card--active': achievement.id === selectedAchievementId },
        ]"
        :aria-pressed="achievement.id === selectedAchievementId"
        @click="handleSelectAchievement(achievement.id)"
      >
        <div class="achievements-page__card-head">
          <h3>{{ achievement.name }}</h3>
          <span>{{ achievement.rarity }}</span>
        </div>
        <p>{{ achievement.description }}</p>
        <div class="achievements-page__tags">
          <span v-for="tag in achievement.tags" :key="tag">{{ tag }}</span>
        </div>
        <div class="achievements-page__progress">
          <div class="achievements-page__progress-fill" :style="{ width: `${achievement.progress}%` }" />
        </div>
        <footer>
          <strong>{{ achievement.progress }}%</strong>
          <small v-if="achievement.limited">{{ achievement.timeRemaining }} left</small>
        </footer>
      </button>
    </section>

    <section v-if="activeAchievement" class="achievements-page__detail">
      <div>
        <p class="achievements-page__detail-title">{{ activeAchievement.name }}</p>
        <span class="achievements-page__detail-desc">{{ activeAchievement.description }}</span>
        <div class="achievements-page__detail-tags">
          <span v-for="tag in activeAchievement.tags" :key="tag">{{ tag }}</span>
        </div>
      </div>
      <div class="achievements-page__detail-meta">
        <strong>{{ activeAchievement.progress }}%</strong>
        <small v-if="activeAchievement.limited">Time left: {{ activeAchievement.timeRemaining }}</small>
        <button type="button">{{ t('achievements.detail.viewRequirements') }}</button>
      </div>
    </section>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.achievements-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
  color: variables.$white;

  &__grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
    gap: 12px;
  }

  &__card {
    background: rgba(255, 255, 255, 0.15);
    padding: 16px;
    border-radius: 18px;
    display: flex;
    flex-direction: column;
    gap: 8px;
    border: 1px solid transparent;
    cursor: pointer;
    color: inherit;
    font: inherit;

    &--active {
      border-color: rgba(255, 255, 255, 0.6);
      box-shadow: 0 6px 20px rgba(0, 0, 0, 0.25);
    }

    &--common {
      border: 1px solid rgba(255, 255, 255, 0.15);
    }

    &--rare {
      border: 1px solid rgba(91, 201, 255, 0.6);
    }

    &--epic {
      border: 1px solid rgba(160, 123, 255, 0.6);
    }

    &--mythic {
      border: 1px solid rgba(255, 179, 71, 0.6);
    }
  }

  &__card-head {
    display: flex;
    justify-content: space-between;

    h3 {
      margin: 0;
      font-size: 16px;
    }

    span {
      text-transform: uppercase;
      font-size: 11px;
    }
  }

  p {
    margin: 0;
    font-size: 13px;
  }

  &__tags {
    display: flex;
    gap: 6px;
    flex-wrap: wrap;

    span {
      font-size: 10px;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      border-radius: 999px;
      padding: 2px 8px;
      border: 1px solid currentColor;
    }
  }

  &__progress {
    position: relative;
    height: 8px;
    border-radius: 999px;
    background: rgba(255, 255, 255, 0.2);
    overflow: hidden;
  }

  &__progress-fill {
    position: absolute;
    inset: 0;
    border-radius: 999px;
    background: linear-gradient(90deg, #42e695, #3bb2b8);
  }

  footer {
    display: flex;
    justify-content: space-between;
    align-items: center;

    strong {
      font-size: 16px;
    }

    small {
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 0.04em;
    }
  }
  &__detail {
    display: flex;
    justify-content: space-between;
    gap: 16px;
    background: rgba(0, 0, 0, 0.25);
    padding: 18px;
    border-radius: 18px;
    flex-wrap: wrap;
  }

  &__detail-title {
    margin: 0 0 6px;
    font-size: 20px;
    font-weight: 700;
  }

  &__detail-desc {
    display: block;
    margin-bottom: 8px;
  }

  &__detail-tags {
    display: flex;
    gap: 8px;
    flex-wrap: wrap;

    span {
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      border-radius: 999px;
      border: 1px solid rgba(255, 255, 255, 0.3);
      padding: 2px 10px;
    }
  }

  &__detail-meta {
    display: flex;
    flex-direction: column;
    gap: 6px;
    align-items: flex-end;

    button {
      padding: 8px 14px;
      border-radius: 999px;
      border: none;
      font-weight: 700;
      cursor: pointer;
    }
  }
}

@media (max-width: 768px) {
  .achievements-page__detail {
    flex-direction: column;

    &-meta {
      align-items: flex-start;
    }
  }
}
</style>