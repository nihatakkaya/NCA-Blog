<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { Menu, Search, X } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'
import { useNotificationStore } from '@/stores/notification'
const auth = useAuthStore()
const router = useRouter()
const toast = useNotificationStore()
const open = ref(false)
const query = ref('')
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
      <RouterLink to="/" class="brand" aria-label="NCA ana sayfa"
        ><img src="/nca-logo.png" alt="NCA" /></RouterLink
      ><button
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
        <form class="nav-search" role="search" @submit.prevent="search">
          <input
            v-model="query"
            type="search"
            aria-label="Yazılarda ara"
            placeholder="Yazılarda ara…"
          /><button class="icon-button" aria-label="Ara"><Search :size="17" /></button>
        </form>
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
