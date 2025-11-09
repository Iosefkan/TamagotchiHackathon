<script lang="ts" setup>
import { SearchOutlined, BellFilled } from '@ant-design/icons-vue'

import type { User } from '@/modules/user/domain'

interface UiHeaderProps {
  user: User
}

defineProps<UiHeaderProps>()
</script>

<template>
  <div class="ui-header">
    <div class="ui-header__user">
      <img :src="user.picture" alt="" />
    </div>
    <div class="ui-header__actions">
      <div class="ui-header__actions-item"><SearchOutlined /></div>
      <div class="ui-header__actions-item"><BellFilled /></div>
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
      font-size: 20px;
      cursor: pointer;
    }
  }
}
</style>