<script setup>
defineProps({ name: { type: String, required: true } })
</script>
<template>
  <span class="tag">{{ name }}</span>
</template>
