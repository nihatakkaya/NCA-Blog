<script setup>
import { formatDate } from '@/utils/date'
defineProps({ post: { type: Object, required: true } })
</script>
<template>
  <div class="post-meta">
    <span>{{ post.authorName }}</span
    ><span aria-hidden="true">·</span
    ><time :datetime="post.createdAt">{{ formatDate(post.createdAt) }}</time>
  </div>
</template>
