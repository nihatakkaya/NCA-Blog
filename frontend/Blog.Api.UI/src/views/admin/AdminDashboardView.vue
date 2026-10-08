<script setup>
import { onMounted } from 'vue'
import { postApi } from '@/api/postApi'
import { categoryApi } from '@/api/categoryApi'
import { tagApi } from '@/api/tagApi'
import { useResource } from '@/composables/useResource'
import ResourceState from '@/components/common/ResourceState.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import { formatDate } from '@/utils/date'
const dashboard = useResource(async () => {
  const [posts, categories, tags] = await Promise.all([
    postApi.admin(),
    categoryApi.list(),
    tagApi.list(),
  ])
  return { data: { posts: posts.data, categories: categories.data, tags: tags.data } }
})
onMounted(() => dashboard.reload())
</script>
<template>
  <div class="page-heading">
    <div>
      <span class="eyebrow">YÖNETİM</span>
      <h1>Dashboard</h1>
    </div>
    <RouterLink class="button button--primary" to="/admin/posts/new">+ Yeni Yazı</RouterLink>
  </div>
  <ResourceState
    :loading="dashboard.loading.value"
    :error="dashboard.error.value"
    @retry="dashboard.reload()"
    ><template v-if="dashboard.data.value"
      ><div class="stat-grid">
        <div
          v-for="stat in [
            { label: 'Toplam yazı', count: dashboard.data.value.posts.length },
            { label: 'Toplam kategori', count: dashboard.data.value.categories.length },
            { label: 'Toplam tag', count: dashboard.data.value.tags.length },
          ]"
          :key="stat.label"
          class="stat-card"
        >
          <span>{{ stat.label }}</span
          ><strong>{{ stat.count }}</strong>
        </div>
      </div>
      <section class="panel">
        <h2>Son Yazılar</h2>
        <div
          v-for="post in [...dashboard.data.value.posts]
            .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
            .slice(0, 5)"
          :key="post.id"
          class="recent-row"
        >
          <RouterLink :to="`/admin/posts/${post.id}/edit`">{{ post.title }}</RouterLink
          ><time :datetime="post.createdAt">{{ formatDate(post.createdAt) }}</time>
        </div>
        <EmptyState
          v-if="!dashboard.data.value.posts.length"
          title="Henüz bir yazı yok."
        /></section></template
  ></ResourceState>
</template>
