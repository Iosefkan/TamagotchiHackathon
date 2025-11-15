<script lang="ts" setup>
import { useTamagotchi } from '~/modules/tamagotchi/ui/Tamagotchi/useCases'

const { tamagotchiImage, stage, mood, mutation } = useTamagotchi()
</script>

<template>
  <div
    class="tamagotchi"
    :class="[`tamagotchi--tier-${stage.tier}`]"
    :style="{ '--tamagotchi-aura': mood.aura, '--tamagotchi-intensity': mood.intensity }"
  >
    <div class="tamagotchi__aura" />
    <div class="tamagotchi__body">
      <img :src="tamagotchiImage" alt="Labubu Tamagotchi" />
      <div class="tamagotchi__status">
        <p class="tamagotchi__stage">{{ stage.name }}</p>
        <span class="tamagotchi__descriptor">{{ stage.descriptor }}</span>
        <small class="tamagotchi__velocity">Velocity: {{ stage.velocityLabel }}</small>
      </div>
    </div>
    <div v-if="mutation?.ready" class="tamagotchi__mutation">
      Ready for {{ mutation.season }}
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.tamagotchi {
  position: relative;
  padding: 20px;
  border-radius: 24px;
  background: rgba(255, 255, 255, 0.12);
  overflow: hidden;
  width: 100%;
  max-width: 320px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  align-items: center;

  &__aura {
    position: absolute;
    inset: 10px;
    border-radius: 24px;
    background: var(--tamagotchi-aura, #6be4c7);
    opacity: calc(0.25 + var(--tamagotchi-intensity, 0.5) * 0.35);
    filter: blur(20px);
    pointer-events: none;
  }

  &__body {
    position: relative;
    z-index: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 14px;
  }

  img {
    width: 180px;
    height: 180px;
    object-fit: contain;
    mix-blend-mode: lighten;
  }

  &__status {
    text-align: center;
    color: variables.$white;
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  &__stage {
    font-size: 18px;
    font-weight: 700;
    margin: 0;
  }

  &__descriptor {
    font-size: 12px;
    opacity: 0.9;
  }

  &__velocity {
    font-size: 10px;
    letter-spacing: 0.04em;
    text-transform: uppercase;
  }

  &__mutation {
    margin-top: 14px;
    padding: 6px 12px;
    border-radius: 999px;
    font-size: 10px;
    text-transform: uppercase;
    color: variables.$white;
    border: 1px solid rgba(255, 255, 255, 0.2);
    align-self: center;
  }

  &--tier-guardian {
    box-shadow: 0 10px 40px rgba(155, 123, 255, 0.45);
  }

  &--tier-scout {
    box-shadow: 0 10px 30px rgba(107, 228, 199, 0.35);
  }

  &--tier-cocoon {
    box-shadow: 0 10px 25px rgba(255, 159, 122, 0.35);
  }
}
</style>