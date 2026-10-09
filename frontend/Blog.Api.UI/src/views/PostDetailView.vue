<script setup>
import { computed, ref, watch, onMounted, onBeforeUnmount } from 'vue'
import { useRoute } from 'vue-router'
import { ArrowLeft } from 'lucide-vue-next'
import { postApi } from '@/api/postApi'
import { useResource } from '@/composables/useResource'
import ResourceState from '@/components/common/ResourceState.vue'
import PostMeta from '@/components/posts/PostMeta.vue'
import CategoryBadge from '@/components/posts/CategoryBadge.vue'
import TagChip from '@/components/posts/TagChip.vue'
import CommentList from '@/components/comments/CommentList.vue'
import '@/assets/styles/article.css'
const articleElement = ref(null)
const readingProgress = ref(0)
let frame = null
let observer
let disposed = false
function measureProgress() {
  frame = null
  const article = articleElement.value
  if (!article) {
    readingProgress.value = 0
    return
  }
  const rect = article.getBoundingClientRect()
  const navbarHeight = document.querySelector('.navbar')?.getBoundingClientRect().height || 0
  const start = rect.top + window.scrollY - navbarHeight
  const end = rect.bottom + window.scrollY - window.innerHeight
  readingProgress.value =
    end <= start
      ? rect.bottom <= window.innerHeight
        ? 1
        : 0
      : Math.min(1, Math.max(0, (window.scrollY - start) / (end - start)))
}
function scheduleProgress() {
  if (disposed) return
  if (frame === null) frame = window.requestAnimationFrame(measureProgress)
}
onMounted(() => {
  observer = new ResizeObserver(scheduleProgress)
  const navbar = document.querySelector('.navbar')
  if (navbar) observer.observe(navbar)
  if (articleElement.value) observer.observe(articleElement.value)
  window.addEventListener('scroll', scheduleProgress, { passive: true })
  window.addEventListener('resize', scheduleProgress)
  scheduleProgress()
})
watch(
  articleElement,
  (current, previous) => {
    if (previous) observer?.unobserve(previous)
    if (current) observer?.observe(current)
    scheduleProgress()
  },
  { flush: 'post' },
)
onBeforeUnmount(() => {
  disposed = true
  observer?.disconnect()
  window.removeEventListener('scroll', scheduleProgress)
  window.removeEventListener('resize', scheduleProgress)
  if (frame !== null) window.cancelAnimationFrame(frame)
})
const route = useRoute()
const post = useResource(() => postApi.detail(route.params.id))
const paragraphs = computed(() =>
  String(post.data.value?.content || '')
    .split(/\r?\n[\t ]*\r?\n/)
    .filter((paragraph) => paragraph.trim()),
)
watch(
  () => route.params.id,
  () => post.reload(),
  { immediate: true },
)
</script>
<template>
  <div class="article-page">
    <div
      v-if="post.data.value && !post.loading.value && !post.error.value"
      class="reading-progress"
      aria-hidden="true"
    >
      <span :style="{ transform: 'scaleX(' + readingProgress + ')' }"></span>
    </div>
    <div class="article-shell">
      <RouterLink class="article-back" to="/"
        ><ArrowLeft :size="16" aria-hidden="true" /> Tüm yazılar</RouterLink
      >
      <ResourceState :loading="post.loading.value" :error="post.error.value" @retry="post.reload()">
        <template v-if="post.data.value">
          <article ref="articleElement" class="post-detail" aria-labelledby="article-title">
            <header class="article-header">
              <CategoryBadge :name="post.data.value.categoryName" />
              <h1 id="article-title">{{ post.data.value.title }}</h1>
              <p v-if="post.data.value.summary" class="detail-summary">
                {{ post.data.value.summary }}
              </p>
              <PostMeta :post="post.data.value" detailed />
              <div
                v-if="post.data.value.tags?.length"
                class="chips article-tags"
                aria-label="Yazının etiketleri"
              >
                <TagChip v-for="tag in post.data.value.tags" :key="tag" :name="tag" />
              </div>
            </header>
            <div class="article-content">
              <p v-for="(paragraph, index) in paragraphs" :key="index" class="article-paragraph">
                {{ paragraph }}
              </p>
            </div>
          </article>
          <CommentList :key="route.params.id" :post-id="route.params.id" />
        </template>
      </ResourceState>
    </div>
  </div>
</template>
