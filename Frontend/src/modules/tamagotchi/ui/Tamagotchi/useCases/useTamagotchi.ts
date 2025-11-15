import { computed } from 'vue'
import TamagotchiDefault from '~/shared/images/tamagotchi/default.webp'
import { useTamagotchiSignals } from '@/modules/tamagotchi/useTamagotchiSignals'

export const useTamagotchi = () => {
  const { tamagotchiStage, tamagotchiMood, seasonalMutation } = useTamagotchiSignals()

  const tamagotchiImage = computed(() => TamagotchiDefault)

  return {
    tamagotchiImage,
    stage: tamagotchiStage,
    mood: tamagotchiMood,
    mutation: seasonalMutation,
  }
}
