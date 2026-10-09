<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { authApi } from '@/api/authApi'
import { apiError } from '@/api/axios'
import { useNotificationStore } from '@/stores/notification'
import AppLogo from '@/components/common/AppLogo.vue'
import BaseInput from '@/components/common/BaseInput.vue'
import BaseButton from '@/components/common/BaseButton.vue'
const router = useRouter()
const toast = useNotificationStore()
const form = reactive({ username: '', email: '', password: '' })
const busy = ref(false)
const error = ref('')
async function submit() {
  if (busy.value) return
  busy.value = true
  error.value = ''
  try {
    await authApi.register({
      username: form.username.trim(),
      email: form.email.trim(),
      password: form.password,
    })
    form.password = ''
    toast.add('Hesabınız oluşturuldu. Giriş yapabilirsiniz.')
    await router.push('/login')
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
      <div class="auth-brand"><AppLogo size="auth" /></div>
      <span class="eyebrow">YENİ HESAP</span>
      <h1>Kayıt Ol</h1>
      <form @submit.prevent="submit">
        <BaseInput
          v-model="form.username"
          label="Kullanıcı adı"
          autocomplete="username"
          required
          :disabled="busy"
        /><BaseInput
          v-model="form.email"
          label="E-posta"
          type="email"
          autocomplete="email"
          required
          :disabled="busy"
        /><BaseInput
          v-model="form.password"
          label="Şifre"
          type="password"
          autocomplete="new-password"
          required
          :disabled="busy"
        />
        <p v-if="error" class="form-error" role="alert">{{ error }}</p>
        <BaseButton type="submit" :loading="busy">Kayıt Ol</BaseButton>
      </form>
      <p class="auth-switch">Hesabın var mı? <RouterLink to="/login">Giriş Yap</RouterLink></p>
    </div>
  </div>
</template>
