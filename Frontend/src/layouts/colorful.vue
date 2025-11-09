<script lang="ts" setup>
</script>

<template>
  <div class="layouts-colorful">
    <div class="layouts-colorful__content">
      <slot />
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

  &__content {
    isolation: isolate;
  }
}
</style>
