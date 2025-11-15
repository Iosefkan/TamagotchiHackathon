<script lang="ts" setup>
import { useTamagotchiSignals } from '@/modules/tamagotchi/useTamagotchiSignals'

const { progressBars, seasonalMutation } = useTamagotchiSignals()
</script>

<template>
  <div class="layouts-colorful">
    <div class="layouts-colorful__container">
      <div v-if="progressBars.length" class="layouts-colorful__hero" aria-live="polite">
        <div class="layouts-colorful__progress">
          <div v-for="bar in progressBars" :key="bar.id" class="layouts-colorful__progress-item">
            <div class="layouts-colorful__progress-top">
              <span>{{ bar.label }}</span>
              <strong>{{ bar.value }}%</strong>
            </div>
            <div class="layouts-colorful__progress-rail" role="progressbar" :aria-valuenow="bar.value" aria-valuemin="0" aria-valuemax="100">
              <div class="layouts-colorful__progress-fill" :class="`layouts-colorful__progress-fill--${bar.tone}`" :style="{ width: `${bar.value}%` }" />
            </div>
            <small>{{ bar.description }}</small>
          </div>
        </div>
        <div class="layouts-colorful__toast">
          <div class="layouts-colorful__toast-indicator" :style="{ background: seasonalMutation.aura }" />
          <div class="layouts-colorful__toast-body">
            <p>{{ seasonalMutation.season }}</p>
            <span>{{ seasonalMutation.summary }}</span>
          </div>
          <span class="layouts-colorful__toast-requirement">{{ seasonalMutation.requirement }}</span>
        </div>
      </div>
      <main class="layouts-colorful__content">
        <slot />
      </main>
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.layouts-colorful {
  position: relative;
  padding: 20px;
  min-height: 100vh;
  background: variables.$vtb-linear-background;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    background: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='100%25' height='100%25'%3E%3Cfilter id='noise'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.90' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noise)' opacity='0.3'/%3E%3C/svg%3E");
    pointer-events: none;
  }

  &__container {
    position: relative;
    z-index: 1;
    max-width: 1100px;
    margin: 0 auto;
    display: flex;
    flex-direction: column;
    gap: 20px;
  }

  &__hero {
    isolation: isolate;
    display: flex;
    flex-direction: column;
    gap: 12px;
    padding: 12px;
    border-radius: 16px;
    background: rgba(0, 0, 0, 0.15);
    backdrop-filter: blur(6px);
  }

  &__progress {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
    gap: 12px;
  }

  &__progress-item {
    padding: 12px;
    border-radius: 14px;
    background: rgba(255, 255, 255, 0.12);
    color: variables.$white;
  }

  &__progress-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 8px;

    span {
      font-size: 12px;
      text-transform: uppercase;
      letter-spacing: 0.04em;
    }

    strong {
      font-size: 16px;
    }
  }

  &__progress-rail {
    position: relative;
    height: 6px;
    border-radius: 999px;
    background: rgba(255, 255, 255, 0.2);
    overflow: hidden;
  }

  &__progress-fill {
    position: absolute;
    inset: 0;
    border-radius: 999px;
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

  &__toast {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 12px 14px;
    border-radius: 14px;
    background: rgba(0, 0, 0, 0.35);
    color: variables.$white;
    flex-wrap: wrap;
  }

  &__toast-indicator {
    width: 38px;
    height: 38px;
    border-radius: 10px;
  }

  &__toast-body {
    display: flex;
    flex-direction: column;
    min-width: 140px;

    p {
      margin: 0;
      font-weight: 700;
    }

    span {
      font-size: 12px;
      opacity: 0.9;
    }
  }

  &__toast-requirement {
    font-size: 11px;
    text-transform: uppercase;
    letter-spacing: 0.04em;
    margin-left: auto;
  }

  &__content {
    isolation: isolate;
    width: 100%;
  }
}

@media (max-width: 768px) {
  .layouts-colorful {
    padding: 16px;

    &__container {
      gap: 16px;
    }
  }
}
</style>
