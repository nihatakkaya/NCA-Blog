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
  <section class="comments-section" aria-labelledby="comments-title">
    <div class="comments-header">
      <div>
        <h2 id="comments-title">Yorumlar</h2>
        <p>Bu yazı hakkındaki düşünceler.</p>
      </div>
      <span v-if="!comments.loading.value && !comments.error.value" class="comment-count"
        >{{ comments.data.value.length }} yorum</span
      >
    </div>
    <ResourceState
      :loading="comments.loading.value"
      :error="comments.error.value"
      @retry="comments.reload()"
      ><CommentItem
        v-for="comment in comments.data.value"
        :key="comment.id"
        :comment="comment"
      /><EmptyState v-if="!comments.data.value.length" title="Henüz yorum yok."
        ><p class="comment-empty-note">
          İlk yorum için aşağıdaki alanı kullanabilirsiniz.
        </p></EmptyState
      ></ResourceState
    ><CommentForm :post-id="postId" @created="comments.reload()" />
  </section>
</template>
