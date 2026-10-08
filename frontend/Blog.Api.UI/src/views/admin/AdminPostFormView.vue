<script setup>
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { postApi } from '@/api/postApi'
import { categoryApi } from '@/api/categoryApi'
import { tagApi } from '@/api/tagApi'
import { apiError } from '@/api/axios'
import { useNotificationStore } from '@/stores/notification'
import { useResource } from '@/composables/useResource'
import BaseInput from '@/components/common/BaseInput.vue'
import BaseButton from '@/components/common/BaseButton.vue'
import ResourceState from '@/components/common/ResourceState.vue'
import TagInput from '@/components/posts/TagInput.vue'
const route = useRoute()
const router = useRouter()
const toast = useNotificationStore()
const editing = computed(() => Boolean(route.params.id))
const form = reactive({ title: '', summary: '', content: '', categoryId: null, tags: [] })
const categories = ref([])
const tags = ref([])
const busy = ref(false)
const error = ref('')
const tagInput = ref(null)
const categoryResolved = ref(true)
const resources = useResource(async () => {
  const id = route.params.id
  const [categoryResult, tagResult, postResult] = await Promise.all([
    categoryApi.list(),
    tagApi.list(),
    id ? postApi.admin() : Promise.resolve({ data: [] }),
  ])
  const post = id ? postResult.data.find((item) => String(item.id) === String(id)) : null
  if (id && !post) throw { response: { status: 404 } }
  return { data: { categories: categoryResult.data, tags: tagResult.data, post } }
})
async function load() {
  const id = route.params.id
  Object.assign(form, { title: '', summary: '', content: '', categoryId: null, tags: [] })
  error.value = ''
  categoryResolved.value = true
  await resources.reload()
  if (resources.error.value || id !== route.params.id || !resources.data.value) return
  const data = resources.data.value
  categories.value = data.categories
  tags.value = data.tags
  if (data.post) {
    const post = data.post
    const category = categories.value.find((item) => item.name === post.categoryName)
    categoryResolved.value = !post.categoryName || Boolean(category)
    Object.assign(form, {
      title: post.title,
      content: post.content,
      summary: post.summary || '',
      categoryId: category?.id ?? null,
      tags: [...(post.tags || [])],
    })
  }
}
watch(() => route.params.id, load, { immediate: true })
async function submit() {
  if (busy.value) return
  tagInput.value?.commit()
  if (!form.title.trim() || !form.content.trim()) {
    error.value = 'Başlık ve içerik boş olamaz.'
    return
  }
  if (!categoryResolved.value) {
    error.value = 'Lütfen kategoriyi açıkça seçin veya “Kategori yok” seçeneğini onaylayın.'
    return
  }
  busy.value = true
  error.value = ''
  const body = {
    title: form.title.trim(),
    summary: form.summary.trim(),
    content: form.content,
    categoryId: form.categoryId,
    tags: [...form.tags],
  }
  try {
    if (editing.value) await postApi.update(route.params.id, body)
    else await postApi.create(body)
    toast.add(editing.value ? 'Yazı güncellendi.' : 'Yazı oluşturuldu.')
    await router.push('/admin/posts')
  } catch (cause) {
    error.value = apiError(cause)
  } finally {
    busy.value = false
  }
}
</script>
<template>
  <div class="page-heading">
    <div>
      <span class="eyebrow">İÇERİK YÖNETİMİ</span>
      <h1>{{ editing ? 'Yazıyı Düzenle' : 'Yeni Yazı' }}</h1>
    </div>
    <RouterLink class="back-link" to="/admin/posts">Yazılara dön</RouterLink>
  </div>
  <ResourceState :loading="resources.loading.value" :error="resources.error.value" @retry="load"
    ><form class="panel post-form" @submit.prevent="submit">
      <fieldset :disabled="busy">
        <BaseInput v-model="form.title" label="Başlık" required />
        <div class="field">
          <label for="post-summary">Özet <span class="muted">(isteğe bağlı)</span></label
          ><textarea id="post-summary" v-model="form.summary" rows="3"></textarea>
        </div>
        <div class="field">
          <label for="post-content">İçerik</label
          ><textarea id="post-content" v-model="form.content" rows="15" required></textarea
          ><small>İçerik düz metin olarak gösterilir.</small>
        </div>
        <div class="field">
          <label for="post-category">Kategori</label
          ><select id="post-category" v-model="form.categoryId" @change="categoryResolved = true">
            <option :value="null">Kategori yok</option>
            <option v-for="category in categories" :key="category.id" :value="category.id">
              {{ category.name }}
            </option></select
          ><template v-if="!categoryResolved"
            ><p class="form-error">
              Yazının kategorisi listede bulunamadı. Yeni seçim yapın veya kategoriyi kaldırın.
            </p>
            <button type="button" class="text-button" @click="categoryResolved = true">
              Kategori yok seçimini onayla
            </button></template
          >
        </div>
        <TagInput ref="tagInput" v-model="form.tags" :suggestions="tags" />
        <p v-if="error" class="form-error" role="alert">{{ error }}</p>
        <div class="actions">
          <BaseButton type="submit" :loading="busy">{{
            editing ? 'Değişiklikleri Kaydet' : 'Yazıyı Oluştur'
          }}</BaseButton
          ><RouterLink class="button button--secondary" to="/admin/posts">Vazgeç</RouterLink>
        </div>
      </fieldset>
    </form></ResourceState
  >
</template>
