<script lang="ts" setup>
import { computed, onBeforeUnmount, ref, watchEffect } from 'vue'
import UiHeader from '@/shared/ui/UiHeader/UiHeader.vue'
import { useFinanceSummary } from '@/modules/dashboard/useFinanceSummary'
import type { User } from '@/modules/user/domain'
import { useTamagotchiSignals } from '@/modules/tamagotchi/useTamagotchiSignals'
import { useI18n } from '@/modules/i18n/useI18n'

definePageMeta({
  layout: 'colorful',
})

const { user } = useFinanceSummary()
const { merchInventory } = useTamagotchiSignals()
const { t } = useI18n()

const headerUser = computed<User>(() => ({
  id: user.value?.id ?? 'placeholder-user',
  name: user.value?.name ?? 'Labubu',
  picture: user.value?.picture ?? null,
}))

const selectedItemId = ref<string | null>(null)
const fusionToast = ref('')
let fusionTimer: ReturnType<typeof setTimeout> | null = null

watchEffect(() => {
  if (!merchInventory.value.length || selectedItemId.value) return
  selectedItemId.value = merchInventory.value[0].id
})

const selectedItem = computed(() => merchInventory.value.find((item) => item.id === selectedItemId.value) ?? null)

const selectItem = (itemId: string) => {
  selectedItemId.value = itemId
}

const triggerFusion = () => {
  if (!selectedItem.value) return
  fusionToast.value = `${selectedItem.value.name} fusion initiated`
  if (fusionTimer) clearTimeout(fusionTimer)
  fusionTimer = setTimeout(() => {
    fusionToast.value = ''
  }, 2600)
}

onBeforeUnmount(() => {
  if (fusionTimer) clearTimeout(fusionTimer)
})
</script>

<template>
  <div class="inventory-page">
    <UiHeader :user="headerUser" />

    <section class="inventory-page__grid">
      <button
        v-for="item in merchInventory"
        :key="item.id"
        type="button"
        class="inventory-page__card"
        :class="{ 'inventory-page__card--active': item.id === selectedItemId }"
        @click="selectItem(item.id)"
      >
        <header>
          <div>
            <h3>{{ item.name }}</h3>
            <span>{{ item.rarity }} • {{ item.slot }} slot</span>
          </div>
          <strong>{{ item.effect }}</strong>
        </header>
        <p>{{ item.perk }}</p>
        <div class="inventory-page__stats">
          <div>
            <small>{{ t('inventory.copies') }}</small>
            <span>{{ item.copies }}</span>
          </div>
          <div>
            <small>{{ t('inventory.duplicates') }}</small>
            <span>{{ item.duplicates }}</span>
          </div>
          <div>
            <small>{{ t('inventory.fusionCost') }}</small>
            <span>{{ item.exchangeCost }}</span>
          </div>
        </div>
        <footer>
          <span :class="['inventory-page__status', { 'inventory-page__status--ready': item.fusionReady }]">
            {{ item.fusionReady ? t('inventory.readyToFuse') : t('inventory.needMoreShards') }}
          </span>
        </footer>
      </button>
    </section>

    <section v-if="selectedItem" class="inventory-page__drawer">
      <div>
        <p class="inventory-page__drawer-title">{{ selectedItem.name }}</p>
        <span class="inventory-page__drawer-desc">{{ selectedItem.effect }}</span>
        <small>{{ t('inventory.slotLabel') }}: {{ selectedItem.slot }} • {{ t('inventory.rarityLabel') }}: {{ selectedItem.rarity }}</small>
      </div>
      <button
        type="button"
        class="inventory-page__button"
        :disabled="!selectedItem.fusionReady"
        @click="triggerFusion"
      >
        {{ selectedItem.fusionReady ? t('inventory.fuseAction') : t('inventory.collectMore') }}
      </button>
    </section>

    <div v-if="fusionToast" class="inventory-page__toast" aria-live="assertive">
      {{ fusionToast }}
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.inventory-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
  color: variables.$white;

  &__grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 12px;
  }

  &__card {
    background: rgba(255, 255, 255, 0.15);
    border-radius: 18px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 12px;
    border: 1px solid transparent;
    cursor: pointer;
    color: inherit;
    font: inherit;
    text-align: left;

    &--active {
      border-color: rgba(255, 255, 255, 0.6);
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.25);
    }

    header {
      display: flex;
      flex-direction: column;
      gap: 6px;

      h3 {
        margin: 0;
        font-size: 18px;
      }

      span {
        font-size: 12px;
        text-transform: uppercase;
        letter-spacing: 0.04em;
      }

      strong {
        font-size: 12px;
        font-weight: 700;
      }
    }
  }

  &__stats {
    display: flex;
    justify-content: space-between;

    div {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    small {
      text-transform: uppercase;
      font-size: 10px;
      letter-spacing: 0.04em;
    }

    span {
      font-size: 14px;
      font-weight: 700;
    }
  }

  &__status {
    font-size: 12px;
    text-transform: uppercase;
    letter-spacing: 0.04em;

    &--ready {
      color: #7fffca;
    }
  }

  &__drawer {
    background: variables.$white;
    color: variables.$black;
    border-radius: 20px;
    padding: 18px;
    display: flex;
    justify-content: space-between;
    gap: 12px;
    flex-wrap: wrap;
    align-items: center;
  }

  &__drawer-title {
    margin: 0 0 4px;
    font-size: 20px;
    font-weight: 700;
  }

  &__drawer-desc {
    display: block;
    margin-bottom: 4px;
  }

  &__button {
    min-width: 200px;
    padding: 12px 20px;
    border-radius: 999px;
    border: none;
    font-weight: 700;
    background: variables.$black;
    color: variables.$white;
    cursor: pointer;

    &:disabled {
      opacity: 0.4;
      cursor: not-allowed;
    }
  }

  &__toast {
    position: fixed;
    bottom: 20px;
    left: 50%;
    transform: translateX(-50%);
    padding: 10px 16px;
    border-radius: 999px;
    background: rgba(0, 0, 0, 0.85);
    color: variables.$white;
  }
}

@media (max-width: 768px) {
  .inventory-page__drawer {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
