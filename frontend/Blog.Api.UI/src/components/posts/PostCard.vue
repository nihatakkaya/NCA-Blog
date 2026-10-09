<script setup>
import { ArrowUpRight } from 'lucide-vue-next'
import PostMeta from './PostMeta.vue'
import CategoryBadge from './CategoryBadge.vue'
import TagChip from './TagChip.vue'
defineProps({ post: { type: Object, required: true } })
</script>
<template>
  <article class="post-card">
    <div>
      <CategoryBadge :name="post.categoryName" />
      <h3>
        <RouterLink :to="`/post/${post.id}`">{{ post.title }}</RouterLink>
      </h3>
      <p class="post-summary">{{ post.summary }}</p>
      <div class="chips"><TagChip v-for="tag in post.tags" :key="tag" :name="tag" /></div>
    </div>
    <div class="post-card-footer">
      <PostMeta :post="post" /><RouterLink
        class="read-link"
        :to="`/post/${post.id}`"
        :aria-label="`${post.title}: Devamını oku`"
        >Devamını oku <ArrowUpRight :size="17"
      /></RouterLink>
    </div>
  </article>
</template>
