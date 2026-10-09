<script setup>
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useNotificationStore } from '@/stores/notification'
import { commentApi } from '@/api/commentApi'
import { apiError } from '@/api/axios'
import BaseButton from '@/components/common/BaseButton.vue'
const props = defineProps({ postId: { type: [String, Number], required: true } })
const emit = defineEmits(['created'])
const auth = useAuthStore()
const toast = useNotificationStore()
const route = useRoute()
const content = ref('')
const busy = ref(false)
const error = ref('')
async function submit() {
  if (!content.value.trim() || busy.value) return
  busy.value = true
  error.value = ''
  try {
    await commentApi.create(props.postId, { content: content.value.trim() })
    content.value = ''
    toast.add('Yorumunuz eklendi.')
    emit('created')
  } catch (cause) {
    error.value = apiError(cause)
  } finally {
    busy.value = false
  }
}
</script>
<template>
  <form v-if="auth.isAuthenticated" class="comment-form" @submit.prevent="submit">
    <h3>Yorum bırak</h3>
    <label for="comment-content">Yorumunuz</label
    ><textarea
      id="comment-content"
      v-model="content"
      rows="4"
      required
      placeholder="Yorumunuzu yazın…"
      :disabled="busy"
    ></textarea>
    <p v-if="error" class="form-error" role="alert">{{ error }}</p>
    <div class="comment-form-actions">
      <BaseButton type="submit" :loading="busy" :disabled="!content.trim()"
        >Yorumu Gönder</BaseButton
      >
    </div>
  </form>
  <div v-else class="comment-login">
    <div>
      <strong>Düşüncelerinizi paylaşın</strong>
      <p>Giriş yaparak yorum yazabilirsiniz.</p>
    </div>
    <RouterLink
      class="button button--secondary"
      :to="{ path: '/login', query: { redirect: route.fullPath } }"
      >Giriş Yap</RouterLink
    >
  </div>
</template>
