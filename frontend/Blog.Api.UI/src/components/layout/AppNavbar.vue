<script setup>
import { ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Menu, Search, X } from 'lucide-vue-next'
import AppLogo from '@/components/common/AppLogo.vue'
import { useAuthStore } from '@/stores/auth'
import { useNotificationStore } from '@/stores/notification'
const auth = useAuthStore()
const router = useRouter()
const toast = useNotificationStore()
const open = ref(false)
const route = useRoute()
const query = ref('')
watch(
  () => route.query.q,
  (value) => {
    query.value = String(value || '')
  },
  { immediate: true },
)
async function logout() {
  try {
    await auth.logout()
  } catch {
    toast.add('Sunucuya çıkış bildirilemedi; yerel oturum kapatıldı.', 'info')
  }
  await router.push('/')
  open.value = false
}
function search() {
  router.push({
    path: '/',
    query: query.value.trim() ? { q: query.value.trim() } : {},
    hash: '#posts',
  })
  open.value = false
}
</script>
<template>
  <header class="navbar">
    <div class="container nav-inner">
      <RouterLink to="/" class="nav-brand" aria-label="NCA ana sayfa"><AppLogo /></RouterLink>
      <form class="nav-search" role="search" @submit.prevent="search">
        <Search :size="18" aria-hidden="true" />
        <input
          v-model="query"
          type="search"
          aria-label="Yazılarda ara"
          placeholder="Yazılarda ara…"
        />
        <button class="icon-button" aria-label="Aramayı gönder">
          <Search :size="17" aria-hidden="true" />
        </button>
      </form>
      <button
        class="icon-button mobile-menu"
        :aria-expanded="open"
        aria-controls="main-nav"
        aria-label="Menüyü aç veya kapat"
        @click="open = !open"
      >
        <X v-if="open" /><Menu v-else />
      </button>
      <nav id="main-nav" :class="{ 'is-open': open }" aria-label="Ana menü">
        <RouterLink to="/" @click="open = false">Ana Sayfa</RouterLink>

        <template v-if="auth.isAuthenticated"
          ><span class="nav-username">{{ auth.user.username }}</span
          ><RouterLink to="/profile" @click="open = false">Profil</RouterLink
          ><RouterLink v-if="auth.isAdmin" to="/admin" @click="open = false">Yönetim</RouterLink
          ><button class="text-button" @click="logout">Çıkış</button></template
        ><template v-else
          ><RouterLink to="/login" @click="open = false">Giriş Yap</RouterLink
          ><RouterLink class="button button--primary" to="/register" @click="open = false"
            >Kayıt Ol</RouterLink
          ></template
        >
      </nav>
    </div>
  </header>
</template>
