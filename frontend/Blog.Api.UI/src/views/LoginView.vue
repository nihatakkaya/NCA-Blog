<script setup>
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useNotificationStore } from '@/stores/notification'
import { apiError } from '@/api/axios'
import BaseInput from '@/components/common/BaseInput.vue'
import BaseButton from '@/components/common/BaseButton.vue'
const auth = useAuthStore()
const toast = useNotificationStore()
const router = useRouter()
const route = useRoute()
const email = ref('')
const password = ref('')
const error = ref('')
const busy = ref(false)
async function submit() {
  if (busy.value) return
  busy.value = true
  error.value = ''
  try {
    await auth.login({ email: email.value.trim(), password: password.value })
    password.value = ''
    toast.add('Giriş başarılı.')
    const redirect = route.query.redirect
    await router.push(
      typeof redirect === 'string' && redirect.startsWith('/') && !redirect.startsWith('//')
        ? redirect
        : '/',
    )
  } catch (cause) {
    error.value = apiError(cause)
  } finally {
    busy.value = false
  }
}
</script>
<template>
  <div class="auth-page">
    <div class="auth-card">
      <img class="auth-logo" src="/nca-logo.png" alt="NCA" /><span class="eyebrow">HESABINIZ</span>
      <h1>Giriş Yap</h1>
      <form @submit.prevent="submit">
        <BaseInput
          v-model="email"
          label="E-posta"
          type="email"
          autocomplete="email"
          required
          :disabled="busy"
        /><BaseInput
          v-model="password"
          label="Şifre"
          type="password"
          autocomplete="current-password"
          required
          :disabled="busy"
        />
        <p v-if="error" class="form-error" role="alert">{{ error }}</p>
        <BaseButton type="submit" :loading="busy">Giriş Yap</BaseButton>
      </form>
      <p class="auth-switch">Hesabın yok mu? <RouterLink to="/register">Kayıt Ol</RouterLink></p>
    </div>
  </div>
</template>
