<script setup>
defineProps({
  loading: Boolean,
  variant: { type: String, default: 'primary' },
  type: { type: String, default: 'button' },
})
</script>
<template>
  <button
    :type="type"
    class="button"
    :class="`button--${variant}`"
    :disabled="loading"
    :aria-busy="loading"
  >
    <span v-if="loading" class="spinner" aria-hidden="true"></span><slot />
  </button>
</template>
