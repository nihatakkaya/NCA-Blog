<script setup>
defineProps({ name: { type: String, default: '' } })
</script>
<template>
  <span v-if="name" class="category-badge">{{ name }}</span>
</template>
