<script lang="ts" setup>
import BankIcon from '@/shared/images/bank.webp'

interface BankIconProps {
  color?: string
  size?: 'small' | 'medium'
}

withDefaults(defineProps<BankIconProps>(), {
  color: '#E7EAF1',
  size: 'medium',
})
</script>

<template>
  <div :class="['bank-icon', `bank-icon--size-${size}`]">
    <div class="bank-icon__image">
      <img :src="BankIcon" alt="" />
    </div>
  </div>
</template>

<style lang="scss" scoped>
.bank-icon {
  display: flex;
  align-items: center;
  justify-content: center;

  background: v-bind(color);

  &--size {
    &-small {
      width: 20px;
      height: 20px;
      border-radius: 4px;

      .bank-icon__image {
        width: 10px;
        height: 10px;
      }
    }

    &-medium {
      width: 30px;
      height: 30px;
      border-radius: 8px;

      .bank-icon__image {
        width: 14px;
        height: 14px;
      }
    }
  }

  img {
    width: 100%;
    height: 100%;
    object-fit: contain;
  }
}
</style>
