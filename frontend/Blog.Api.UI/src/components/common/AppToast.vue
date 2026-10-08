<script setup>
import { useNotificationStore } from '@/stores/notification'
const notifications = useNotificationStore()
</script>
<template>
  <div class="toasts" aria-live="polite" aria-atomic="false">
    <div
      v-for="item in notifications.items"
      :key="item.id"
      class="toast"
      :class="`toast--${item.type}`"
    >
      <span>{{ item.message }}</span
      ><button
        class="icon-button"
        aria-label="Bildirimi kapat"
        @click="notifications.remove(item.id)"
      >
        ×
      </button>
    </div>
  </div>
</template>
