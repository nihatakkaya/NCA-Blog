<script setup>
import { UserRound, CalendarDays } from 'lucide-vue-next'
import { formatDate } from '@/utils/date'
defineProps({ post: { type: Object, required: true }, detailed: Boolean })
</script>
<template>
  <div class="post-meta" :class="{ 'post-meta--detailed': detailed }">
    <span class="meta-author"
      ><UserRound v-if="detailed" :size="16" aria-hidden="true" /><span>{{
        post.authorName
      }}</span></span
    >
    <span class="meta-separator" aria-hidden="true">·</span>
    <time :datetime="post.createdAt"
      ><CalendarDays v-if="detailed" :size="16" aria-hidden="true" />{{
        formatDate(post.createdAt)
      }}</time
    >
  </div>
</template>
