<script setup>
import { watch } from 'vue'
import { useRoute } from 'vue-router'
import { ArrowLeft } from 'lucide-vue-next'
import { postApi } from '@/api/postApi'
import { useResource } from '@/composables/useResource'
import ResourceState from '@/components/common/ResourceState.vue'
import PostMeta from '@/components/posts/PostMeta.vue'
import CategoryBadge from '@/components/posts/CategoryBadge.vue'
import TagChip from '@/components/posts/TagChip.vue'
import CommentList from '@/components/comments/CommentList.vue'
const route = useRoute()
const post = useResource(() => postApi.detail(route.params.id))
watch(
  () => route.params.id,
  () => post.reload(),
  { immediate: true },
)
</script>
<template>
  <div class="reading-container">
    <RouterLink class="back-link" to="/"><ArrowLeft :size="16" /> Tüm yazılar</RouterLink
    ><ResourceState :loading="post.loading.value" :error="post.error.value" @retry="post.reload()"
      ><template v-if="post.data.value"
        ><article class="post-detail">
          <CategoryBadge :name="post.data.value.categoryName" />
          <h1>{{ post.data.value.title }}</h1>
          <p v-if="post.data.value.summary" class="detail-summary">{{ post.data.value.summary }}</p>
          <PostMeta :post="post.data.value" />
          <div class="chips">
            <TagChip v-for="tag in post.data.value.tags" :key="tag" :name="tag" />
          </div>
          <div class="article-content plain-content">{{ post.data.value.content }}</div>
        </article>
        <CommentList :key="route.params.id" :post-id="route.params.id" /></template
    ></ResourceState>
  </div>
</template>
