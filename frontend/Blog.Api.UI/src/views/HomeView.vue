<script setup>
import { ref, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Search, ArrowDown, ArrowLeft, ArrowRight } from 'lucide-vue-next'
import { postApi } from '@/api/postApi'
import { useResource } from '@/composables/useResource'
import ResourceState from '@/components/common/ResourceState.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import PostList from '@/components/posts/PostList.vue'
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
    <section class="hero">
      <div class="hero-copy">
        <span class="eyebrow"><span class="accent-dot"></span> KİŞİSEL BLOG</span>
        <h1>NCA<span class="hero-period">.</span></h1>
        <form class="hero-search" role="search" @submit.prevent="search">
          <Search :size="21" aria-hidden="true" /><input
            v-model="query"
            type="search"
            placeholder="Bir yazı, bir konu ara…"
            aria-label="Blog yazılarında ara"
          /><button class="button button--primary">Ara</button>
        </form>
        <a class="hero-link" href="#posts">Son yazılara git <ArrowDown :size="16" /></a>
      </div>
      <div class="hero-art" aria-hidden="true">
        <div class="orbit orbit-one"></div>
        <div class="orbit orbit-two"></div>
        <div class="orbit orbit-three"></div>
        <div class="art-core"><img src="/nca-logo.png" alt="" /></div>
        <span class="art-dot art-dot-one"></span><span class="art-dot art-dot-two"></span>
      </div>
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
