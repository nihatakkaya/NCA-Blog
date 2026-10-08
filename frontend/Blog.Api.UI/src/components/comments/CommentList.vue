<script setup>
import { onMounted } from 'vue'
import { commentApi } from '@/api/commentApi'
import { useResource } from '@/composables/useResource'
import ResourceState from '@/components/common/ResourceState.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import CommentItem from './CommentItem.vue'
import CommentForm from './CommentForm.vue'
const props = defineProps({ postId: { type: [String, Number], required: true } })
const comments = useResource(() => commentApi.list(props.postId), [])
onMounted(() => comments.reload())
</script>
<template>
  <section class="comments-section">
    <h2>Yorumlar</h2>
    <ResourceState
      :loading="comments.loading.value"
      :error="comments.error.value"
      @retry="comments.reload()"
      ><CommentItem
        v-for="comment in comments.data.value"
        :key="comment.id"
        :comment="comment" /><EmptyState
        v-if="!comments.data.value.length"
        title="Henüz yorum yok." /></ResourceState
    ><CommentForm :post-id="postId" @created="comments.reload()" />
  </section>
</template>
