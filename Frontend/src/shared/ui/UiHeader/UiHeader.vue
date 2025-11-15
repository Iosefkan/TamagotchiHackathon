<script lang="ts" setup>
import { computed } from 'vue'
import { SearchOutlined, BellFilled } from '@ant-design/icons-vue'
import type { User } from '@/modules/user/domain'
import UserPictureImage from '@/shared/images/user_pic.webp'
import { useTamagotchiSignals } from '@/modules/tamagotchi/useTamagotchiSignals'

interface UiHeaderProps {
  user: User
}

const props = defineProps<UiHeaderProps>()

const picture = computed(() => props.user.picture ?? UserPictureImage)
const { headerBadges } = useTamagotchiSignals()
</script>

<template>
  <div class="ui-header">
    <div class="ui-header__user">
      <img :src="picture" alt="" />
    </div>
    <div class="ui-header__actions">
      <div class="ui-header__actions-item">
        <SearchOutlined />
        <span v-if="headerBadges.merch" class="ui-header__badge">{{ headerBadges.merch }}</span>
      </div>
      <div class="ui-header__actions-item">
        <BellFilled />
        <span v-if="headerBadges.achievements" class="ui-header__badge">{{ headerBadges.achievements }}</span>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/shared/styles/variables';

.ui-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;

  &__user {
    width: 30px;
    height: 30px;
    border-radius: 50%;
    border: 2px solid variables.$white;

    img {
      width: 100%;
      height: 100%;
      object-fit: cover;
      border-radius: 50%;
    }
  }

  &__actions {
    display: flex;
    align-items: center;
    gap: 10px;

    color: variables.$white;

    &-item {
      position: relative;
      font-size: 20px;
      cursor: pointer;
    }
  }

  &__badge {
    position: absolute;
    top: -4px;
    right: -6px;
    min-width: 16px;
    height: 16px;
    padding: 0 4px;
    border-radius: 999px;
    background: variables.$white;
    color: variables.$black;
    font-size: 10px;
    font-weight: 700;
    line-height: 16px;
    text-align: center;
  }
}
</style>