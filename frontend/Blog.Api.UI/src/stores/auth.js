import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { authApi } from '@/api/authApi'
import { session } from '@/api/session'
export const useAuthStore = defineStore('auth', () => {
  const accessToken = ref(session.access)
  const refreshToken = ref(session.refresh)
  const user = ref(null)
  const initialized = ref(false)
  const restoreError = ref('')
  let restorePromise
  const isAuthenticated = computed(() => Boolean(user.value && accessToken.value))
  const isAdmin = computed(() => isAuthenticated.value && user.value.role === 'Admin')
  function sync() {
    accessToken.value = session.access
    refreshToken.value = session.refresh
    if (!session.access) user.value = null
  }
  window.addEventListener('nca:tokens', sync)
  async function loadUser() {
    user.value = (await authApi.me()).data
  }
  async function login(body) {
    const { data } = await authApi.login(body)
    session.save(data)
    try {
      await loadUser()
      restoreError.value = ''
    } catch (error) {
      session.clear()
      throw error
    }
  }
  async function restore() {
    if (initialized.value) return
    if (!restorePromise)
      restorePromise = (async () => {
        if (session.access || session.refresh) {
          try {
            await loadUser()
            restoreError.value = ''
          } catch (error) {
            if (error.response?.status === 401) session.clear()
            else
              restoreError.value = 'Oturum bilgileri alınamadı. Sunucu bağlantısını kontrol edin.'
          }
        }
        initialized.value = true
      })().finally(() => {
        restorePromise = undefined
      })
    return restorePromise
  }
  async function logout() {
    try {
      if (session.refresh) await authApi.logout(session.refresh)
    } finally {
      session.clear()
      restoreError.value = ''
    }
  }
  return {
    accessToken,
    refreshToken,
    user,
    initialized,
    restoreError,
    isAuthenticated,
    isAdmin,
    login,
    restore,
    logout,
    loadUser,
  }
})
