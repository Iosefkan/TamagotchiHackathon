<script lang="ts" setup>
import { computed, onBeforeUnmount, ref } from 'vue'
import UiHeader from '@/shared/ui/UiHeader/UiHeader.vue'
import Tamagotchi from '@/modules/tamagotchi/ui/Tamagotchi/Tamagotchi.vue'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'
import type { User } from '@/modules/user/domain'
import { useTamagotchiSignals } from '@/modules/tamagotchi/useTamagotchiSignals'
import { useI18n } from '@/modules/i18n/useI18n'

definePageMeta({
  layout: 'colorful',
})

const { user } = useFinanceSummary()
const {
  progressBars,
  tamagotchiStage,
  seasonalMutation,
  miniGames,
  quizQueue,
  nutritionPrompts,
  reactionTrend,
  achievements,
} = useTamagotchiSignals()
const { t } = useI18n()

const headerUser = computed<User>(() => ({
  id: user.value?.id ?? 'placeholder-user',
  name: user.value?.name ?? 'Labubu',
  picture: user.value?.picture ?? null,
}))

const limitedChallenges = computed(() => achievements.value.filter((achievement) => achievement.limited))

type ActionSheet =
  | { type: 'minigame'; title: string; description: string; reward: string }
  | { type: 'quiz'; title: string; description: string; reward: string }
  | { type: 'nutrition'; title: string; description: string }

const selectedAction = ref<ActionSheet | null>(null)
const actionToast = ref('')
let toastTimer: ReturnType<typeof setTimeout> | null = null

const selectMiniGame = (game: (typeof miniGames.value)[number]) => {
  selectedAction.value = {
    type: 'minigame',
    title: game.title,
    description: game.description,
    reward: game.reward,
  }
}

const selectQuiz = (quiz: (typeof quizQueue.value)[number]) => {
  selectedAction.value = {
    type: 'quiz',
    title: quiz.title,
    description: quiz.topic,
    reward: quiz.reward,
  }
}

const selectNutrition = (prompt: (typeof nutritionPrompts.value)[number]) => {
  selectedAction.value = {
    type: 'nutrition',
    title: prompt.goal,
    description: `${prompt.action} • ${prompt.highlight}`,
  }
}

const closeActionSheet = () => {
  selectedAction.value = null
}

const triggerToast = (message: string) => {
  actionToast.value = message
  if (toastTimer) clearTimeout(toastTimer)
  toastTimer = setTimeout(() => {
    actionToast.value = ''
  }, 2600)
}

const launchAction = () => {
  if (!selectedAction.value) return
  triggerToast(`${selectedAction.value.title} queued for Labubu`)
  selectedAction.value = null
}

onBeforeUnmount(() => {
  if (toastTimer) clearTimeout(toastTimer)
})
</script>

<template>
  <div class="tamagotchi-page">
    <UiHeader :user="headerUser" />

    <section class="tamagotchi-page__hero">
      <Tamagotchi class="tamagotchi-page__character" />
      <div class="tamagotchi-page__stage-card">
        <div class="tamagotchi-page__stage-headline">
          <div>
            <h2>{{ tamagotchiStage.name }}</h2>
            <p>{{ tamagotchiStage.descriptor }}</p>
          </div>
          <span>{{ tamagotchiStage.velocityLabel }}</span>
        </div>
        <div class="tamagotchi-page__trend">
          <span v-for="streak in reactionTrend" :key="streak.id" :style="{ backgroundColor: streak.color }" />
          <small>{{ t('tamagotchi.recentStreak') }}</small>
        </div>
        <div class="tamagotchi-page__mutation">
          <p>{{ seasonalMutation.season }}</p>
          <span>{{ seasonalMutation.summary }}</span>
          <strong>{{ seasonalMutation.requirement }}</strong>
        </div>
      </div>
    </section>

    <section class="tamagotchi-page__progress">
      <article v-for="bar in progressBars" :key="bar.id" class="tamagotchi-page__progress-card">
        <div class="tamagotchi-page__progress-top">
          <h3>{{ bar.label }}</h3>
          <span>{{ bar.value }}%</span>
        </div>
        <div class="tamagotchi-page__progress-rail">
          <div class="tamagotchi-page__progress-fill" :class="`tamagotchi-page__progress-fill--${bar.tone}`" :style="{ width: `${bar.value}%` }" />
        </div>
        <p>{{ bar.description }}</p>
      </article>
    </section>

    <section class="tamagotchi-page__minigames">
      <header>
        <h3>{{ t('tamagotchi.miniGames.title') }}</h3>
        <p>{{ t('tamagotchi.miniGames.subtitle') }}</p>
      </header>
      <div class="tamagotchi-page__minigames-grid">
        <button
          v-for="game in miniGames"
          :key="game.id"
          type="button"
          :class="['tamagotchi-page__minigame', `tamagotchi-page__minigame--${game.status}`]"
          :disabled="game.status === 'cooldown'"
          @click="selectMiniGame(game)"
        >
          <div class="tamagotchi-page__minigame-head">
            <h4>{{ game.title }}</h4>
            <span>{{ game.type }}</span>
          </div>
          <p>{{ game.description }}</p>
          <div class="tamagotchi-page__minigame-meta">
            <small>Reward: {{ game.reward }}</small>
            <strong>Boosts {{ game.boostTarget }}</strong>
            <em>{{ game.status === 'available' ? 'Ready to launch' : 'Cooling down' }}</em>
          </div>
        </button>
      </div>
    </section>

    <section class="tamagotchi-page__quiz">
      <header>
        <h3>{{ t('tamagotchi.quiz.title') }}</h3>
        <p>{{ t('tamagotchi.quiz.subtitle') }}</p>
      </header>
      <div class="tamagotchi-page__quiz-grid">
        <button v-for="quiz in quizQueue" :key="quiz.id" type="button" @click="selectQuiz(quiz)">
          <h4>{{ quiz.title }}</h4>
          <p>{{ quiz.topic }}</p>
          <span>Locks onto: {{ quiz.lockingCategory }}</span>
          <strong>{{ quiz.reward }}</strong>
          <small>Boosts {{ quiz.boostTarget }}</small>
        </button>
      </div>
    </section>

    <section class="tamagotchi-page__nutrition">
      <header>
        <h3>{{ t('tamagotchi.nutrition.title') }}</h3>
        <p>{{ t('tamagotchi.nutrition.subtitle') }}</p>
      </header>
      <div class="tamagotchi-page__nutrition-grid">
        <button
          v-for="prompt in nutritionPrompts"
          :key="prompt.goal"
          type="button"
          :class="['tamagotchi-page__nutrition-card', { 'tamagotchi-page__nutrition-card--positive': prompt.onTrack }]"
          @click="selectNutrition(prompt)"
        >
          <h4>{{ prompt.goal }}</h4>
          <p>{{ prompt.status }}</p>
          <strong>{{ prompt.action }}</strong>
          <small>{{ prompt.highlight }}</small>
        </button>
      </div>
    </section>

    <section class="tamagotchi-page__challenges" v-if="limitedChallenges.length">
      <header>
        <h3>{{ t('tamagotchi.challenges.title') }}</h3>
        <p>{{ t('tamagotchi.challenges.subtitle') }}</p>
      </header>
      <div class="tamagotchi-page__challenges-grid">
        <article v-for="challenge in limitedChallenges" :key="challenge.id">
          <div class="tamagotchi-page__challenge-head">
            <h4>{{ challenge.name }}</h4>
            <span>{{ challenge.rarity }}</span>
          </div>
          <p>{{ challenge.description }}</p>
          <div class="tamagotchi-page__challenge-progress">
            <div class="tamagotchi-page__challenge-progress-fill" :style="{ width: `${challenge.progress}%` }" />
          </div>
          <footer>
            <small>{{ challenge.progress }}% complete</small>
            <strong v-if="challenge.timeRemaining">{{ challenge.timeRemaining }} left</strong>
          </footer>
        </article>
      </div>
    </section>

    <div v-if="selectedAction" class="tamagotchi-page__drawer">
      <div>
        <p>{{ selectedAction.title }}</p>
        <small>{{ selectedAction.description }}</small>
        <span v-if="selectedAction.reward">Reward: {{ selectedAction.reward }}</span>
      </div>
      <div class="tamagotchi-page__drawer-actions">
        <button type="button" class="tamagotchi-page__button" @click="launchAction">Launch</button>
        <button type="button" class="tamagotchi-page__button tamagotchi-page__button--ghost" @click="closeActionSheet">
          Close
        </button>
      </div>
    </div>

    <div v-if="actionToast" class="tamagotchi-page__toast" aria-live="assertive">
      {{ actionToast }}
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.tamagotchi-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
  color: variables.$white;
  width: 100%;

  &__hero {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
    gap: 16px;
    align-items: stretch;
  }

  &__stage-card {
    background: rgba(255, 255, 255, 0.15);
    border-radius: 20px;
    padding: 18px;
    display: flex;
    flex-direction: column;
    gap: 16px;
  }

  &__stage-headline {
    display: flex;
    justify-content: space-between;
    gap: 12px;

    h2 {
      margin: 0 0 4px;
      font-size: 20px;
    }

    p {
      margin: 0;
      font-size: 13px;
      opacity: 0.85;
    }

    span {
      font-size: 12px;
      text-transform: uppercase;
    }
  }

  &__trend {
    display: flex;
    align-items: center;
    gap: 6px;

    span {
      width: 14px;
      height: 14px;
      border-radius: 50%;
      opacity: 0.85;
    }

    small {
      margin-left: auto;
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 0.04em;
    }
  }

  &__mutation {
    display: flex;
    flex-direction: column;
    gap: 4px;
    background: rgba(0, 0, 0, 0.25);
    border-radius: 16px;
    padding: 12px;

    p {
      margin: 0;
      font-weight: 700;
    }

    span {
      font-size: 13px;
    }

    strong {
      font-size: 12px;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }
  }

  &__progress {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(190px, 1fr));
    gap: 14px;
  }

  &__progress-card {
    background: variables.$white;
    color: variables.$black;
    padding: 16px;
    border-radius: 18px;
    display: flex;
    flex-direction: column;
    gap: 10px;
  }

  &__progress-top {
    display: flex;
    justify-content: space-between;

    h3 {
      margin: 0;
      font-size: 14px;
      text-transform: uppercase;
      letter-spacing: 0.04em;
    }

    span {
      font-size: 18px;
      font-weight: 700;
    }
  }

  &__progress-rail {
    position: relative;
    height: 10px;
    border-radius: 10px;
    background: rgba(0, 0, 0, 0.08);
    overflow: hidden;
  }

  &__progress-fill {
    position: absolute;
    inset: 0;
    border-radius: 10px;
    transition: width 0.3s ease;

    &--positive {
      background: linear-gradient(90deg, #42e695, #3bb2b8);
    }

    &--warning {
      background: linear-gradient(90deg, #fddb92, #d1fdff);
    }

    &--critical {
      background: linear-gradient(90deg, #ff758c, #ff7eb3);
    }
  }

  &__minigames,
  &__quiz,
  &__nutrition,
  &__challenges {
    background: rgba(255, 255, 255, 0.15);
    border-radius: 20px;
    padding: 18px;

    header {
      margin-bottom: 16px;

      h3 {
        margin: 0;
        text-transform: uppercase;
        letter-spacing: 0.04em;
      }

      p {
        margin: 4px 0 0;
        font-size: 13px;
        opacity: 0.85;
      }
    }
  }

  &__minigames-grid,
  &__quiz-grid,
  &__nutrition-grid,
  &__challenges-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
    gap: 12px;
  }

  &__minigame {
    background: rgba(0, 0, 0, 0.2);
    border-radius: 16px;
    padding: 12px;
    display: flex;
    flex-direction: column;
    gap: 8px;
    text-align: left;
    border: 1px solid transparent;
    transition: border-color 0.2s ease, transform 0.2s ease;
    color: inherit;
    font: inherit;
    cursor: pointer;

    &--cooldown {
      opacity: 0.7;
    }

    &:not(:disabled):hover {
      border-color: rgba(255, 255, 255, 0.35);
      transform: translateY(-2px);
    }
  }

  &__minigame-head {
    display: flex;
    justify-content: space-between;

    h4 {
      margin: 0;
      font-size: 14px;
    }

    span {
      font-size: 11px;
      text-transform: uppercase;
    }
  }

  &__minigame-meta {
    display: flex;
    flex-direction: column;
    gap: 4px;

    strong {
      text-transform: capitalize;
    }

    em {
      font-size: 11px;
      opacity: 0.8;
    }
  }

  &__quiz-grid button,
  &__nutrition-card,
  &__challenges-grid article {
    background: rgba(0, 0, 0, 0.2);
    border-radius: 16px;
    padding: 12px;
    display: flex;
    flex-direction: column;
    gap: 6px;
    border: 1px solid transparent;
    text-align: left;
    transition: border-color 0.2s ease, transform 0.2s ease;
    color: inherit;
    font: inherit;

    &:hover {
      border-color: rgba(255, 255, 255, 0.35);
      transform: translateY(-1px);
    }
  }

  &__nutrition-card--positive {
    border: 1px solid rgba(123, 255, 196, 0.4);
  }

  &__challenge-head {
    display: flex;
    justify-content: space-between;
    align-items: center;

    h4 {
      margin: 0;
      font-size: 14px;
    }

    span {
      font-size: 11px;
      text-transform: uppercase;
    }
  }

  &__challenge-progress {
    position: relative;
    height: 8px;
    border-radius: 999px;
    background: rgba(255, 255, 255, 0.15);
    overflow: hidden;
  }

  &__challenge-progress-fill {
    position: absolute;
    inset: 0;
    border-radius: 999px;
    background: linear-gradient(90deg, #ffb347, #ffcc33);
  }

  &__character {
    min-height: 320px;
  }

  &__drawer {
    background: variables.$white;
    color: variables.$black;
    border-radius: 18px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 10px;
    box-shadow: 0 16px 40px rgba(0, 0, 0, 0.2);
  }

  &__drawer-actions {
    display: flex;
    gap: 10px;
    flex-wrap: wrap;
  }

  &__button {
    flex: 1 1 120px;
    padding: 10px 14px;
    border-radius: 999px;
    border: none;
    font-weight: 700;
    cursor: pointer;

    &--ghost {
      background: transparent;
      border: 1px solid rgba(0, 0, 0, 0.4);
    }
  }

  &__toast {
    position: fixed;
    bottom: 20px;
    left: 50%;
    transform: translateX(-50%);
    background: rgba(0, 0, 0, 0.85);
    color: variables.$white;
    padding: 10px 16px;
    border-radius: 999px;
    font-size: 13px;
    letter-spacing: 0.02em;
  }
}

@media (max-width: 768px) {
  .tamagotchi-page {
    &__hero {
      grid-template-columns: 1fr;
    }

    &__drawer-actions {
      flex-direction: column;
    }
  }
}
</style>