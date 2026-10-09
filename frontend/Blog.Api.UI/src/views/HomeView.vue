<script setup>
import { ref, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ArrowLeft, ArrowRight } from 'lucide-vue-next'
import { postApi } from '@/api/postApi'
import { useResource } from '@/composables/useResource'
import ResourceState from '@/components/common/ResourceState.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import PostList from '@/components/posts/PostList.vue'
import AppLogo from '@/components/common/AppLogo.vue'
const route = useRoute()
const router = useRouter()
const query = ref('')
const posts = useResource(() =>
  route.query.q
    ? postApi.search(String(route.query.q))
    : postApi.list(Math.max(1, Number(route.query.page) || 1)),
)
function loadPosts() {
  query.value = String(route.query.q || '')
  posts.reload()
}
function search() {
  router.push({
    path: '/',
    query: query.value.trim() ? { q: query.value.trim() } : {},
    hash: '#posts',
  })
}
function clearSearch() {
  query.value = ''
  search()
}
function page(number) {
  router.push({ path: '/', query: { page: number }, hash: '#posts' })
}
watch(() => [route.query.q, route.query.page], loadPosts)
onMounted(loadPosts)
</script>
<template>
  <div class="container home-page">
    <section class="hero" aria-labelledby="hero-title">
      <div class="hero-copy">
        <span class="eyebrow"><span class="accent-dot"></span> KİŞİSEL BLOG</span>
        <h1 id="hero-title">Yazılım, teknoloji ve öğrendiklerim üzerine.</h1>
        <p class="hero-description">Öğrenme notlarım, projelerim ve günlük düşüncelerim.</p>
      </div>
      <div class="hero-art" aria-hidden="true"><AppLogo size="hero" decorative /></div>
    </section>
    <section id="posts" class="section">
      <div class="section-heading">
        <div>
          <span class="eyebrow">YAZILAR</span>
          <h2>{{ route.query.q ? 'Arama sonuçları' : 'Son Yazılar' }}</h2>
        </div>
        <button v-if="route.query.q" class="text-button" @click="clearSearch">
          Aramayı temizle</button
        ><span v-else-if="posts.data.value && !posts.error.value" class="muted"
          >{{ posts.data.value.totalCount }} yazı</span
        >
      </div>
      <p v-if="route.query.q" class="muted">“{{ route.query.q }}” için sonuçlar</p>
      <ResourceState
        :loading="posts.loading.value"
        :error="posts.error.value"
        @retry="posts.reload()"
        ><PostList
          v-if="(route.query.q ? posts.data.value : posts.data.value?.items)?.length"
          :posts="route.query.q ? posts.data.value : posts.data.value.items" /><EmptyState
          v-else
          :title="
            route.query.q
              ? 'Aramanızla eşleşen bir yazı bulunamadı.'
              : 'Henüz yayınlanmış bir yazı yok.'
          " />
        <nav
          v-if="!route.query.q && posts.data.value?.totalPages > 1"
          class="pagination"
          aria-label="Yazı sayfaları"
        >
          <button
            class="button button--secondary"
            :disabled="posts.data.value.page <= 1"
            @click="page(posts.data.value.page - 1)"
          >
            <ArrowLeft :size="16" /> Önceki</button
          ><span>{{ posts.data.value.page }} / {{ posts.data.value.totalPages }}</span
          ><button
            class="button button--secondary"
            :disabled="posts.data.value.page >= posts.data.value.totalPages"
            @click="page(posts.data.value.page + 1)"
          >
            Sonraki <ArrowRight :size="16" />
          </button></nav
      ></ResourceState>
    </section>
  </div>
</template>
