<script setup>
import PostCard from './PostCard.vue'
defineProps({ posts: { type: Array, required: true } })
</script>
<template>
  <div class="post-grid"><PostCard v-for="post in posts" :key="post.id" :post="post" /></div>
</template>
