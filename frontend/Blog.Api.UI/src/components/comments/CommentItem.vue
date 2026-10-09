<script setup>
import { computed } from 'vue'
import { formatDate } from '@/utils/date'
const props = defineProps({ comment: { type: Object, required: true } })
const initial = computed(() =>
  Array.from(props.comment.username?.trim() || '?')[0].toLocaleUpperCase('tr'),
)
</script>
<template>
  <article class="comment">
    <span class="comment-avatar" aria-hidden="true">{{ initial }}</span>
    <div class="comment-body">
      <div class="comment-heading">
        <strong>{{ comment.username }}</strong
        ><time :datetime="comment.createdAt">{{ formatDate(comment.createdAt) }}</time>
      </div>
      <p class="plain-content">{{ comment.content }}</p>
    </div>
  </article>
</template>
