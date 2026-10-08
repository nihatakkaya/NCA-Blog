<script setup>
defineProps({ title: { type: String, default: 'Henüz bir kayıt yok.' } })
</script>
<template>
  <div class="empty-state">
    <span class="empty-mark" aria-hidden="true">—</span>
    <p>{{ title }}</p>
    <slot />
  </div>
</template>
