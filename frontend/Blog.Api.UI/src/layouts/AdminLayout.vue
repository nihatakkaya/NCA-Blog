<script setup>
import { ref } from 'vue'
import { LayoutDashboard, FileText, Plus, Folder, Tags, ArrowLeft, Menu } from 'lucide-vue-next'
const open = ref(false)
const links = [
  { path: '/admin', label: 'Dashboard', icon: LayoutDashboard },
  { path: '/admin/posts', label: 'Yazılar', icon: FileText },
  { path: '/admin/posts/new', label: 'Yeni Yazı', icon: Plus },
  { path: '/admin/categories', label: 'Kategoriler', icon: Folder },
  { path: '/admin/tags', label: 'Tagler', icon: Tags },
]
</script>
<template>
  <div class="admin-shell">
    <aside class="admin-sidebar">
      <RouterLink class="brand" to="/"><img src="/nca-logo.png" alt="NCA" /></RouterLink
      ><span class="eyebrow">YÖNETİM PANELİ</span
      ><button
        class="button button--secondary mobile-menu"
        :aria-expanded="open"
        aria-controls="admin-nav"
        @click="open = !open"
      >
        <Menu :size="18" /> Menü
      </button>
      <nav id="admin-nav" :class="{ 'is-open': open }" aria-label="Yönetim">
        <RouterLink
          v-for="link in links"
          :key="link.path"
          :to="link.path"
          exact-active-class="selected"
          @click="open = false"
          ><component :is="link.icon" :size="18" />{{ link.label }}</RouterLink
        ><RouterLink to="/"><ArrowLeft :size="18" />Siteye Dön</RouterLink>
      </nav>
    </aside>
    <main id="main-content" class="admin-content"><RouterView /></main>
  </div>
</template>
