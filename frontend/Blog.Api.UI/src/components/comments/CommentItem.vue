<script setup>
import { formatDate } from '@/utils/date'
defineProps({ comment: { type: Object, required: true } })
</script>
<template>
  <article class="comment">
    <div class="comment-heading">
      <strong>{{ comment.username }}</strong
      ><time :datetime="comment.createdAt">{{ formatDate(comment.createdAt) }}</time>
    </div>
    <p class="plain-content">{{ comment.content }}</p>
  </article>
</template>
