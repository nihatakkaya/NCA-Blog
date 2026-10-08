<script setup>
import LoadingSpinner from './LoadingSpinner.vue'
defineProps({ loading: Boolean, error: { type: String, default: '' } })
defineEmits(['retry'])
</script>
<template>
  <LoadingSpinner v-if="loading" />
  <div v-else-if="error" class="error-panel" role="alert">
    <p>{{ error }}</p>
    <button class="button button--secondary" @click="$emit('retry')">Tekrar dene</button>
  </div>
  <slot v-else />
</template>
