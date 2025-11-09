<script lang="ts" setup>
import { useTamagotchi } from '~/modules/tamagotchi/ui/Tamagotchi/useCases'


const { tamagotchiImage } = useTamagotchi()
</script>

<template>
  <div class="tamagotchi">
    <img :src="tamagotchiImage" alt="" />
  </div>
</template>

<style lang="scss" scoped>
.tamagotchi {
  img {
    width: 100%;
    height: 100%;
    object-fit: contain;
  }
}
</style>