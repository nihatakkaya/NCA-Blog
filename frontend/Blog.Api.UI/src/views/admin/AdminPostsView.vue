<script setup>
import { onMounted, ref } from 'vue'
import { postApi } from '@/api/postApi'
import { apiError } from '@/api/axios'
import { useResource } from '@/composables/useResource'
import { useNotificationStore } from '@/stores/notification'
import { formatDate } from '@/utils/date'
import ResourceState from '@/components/common/ResourceState.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import ConfirmDialog from '@/components/common/ConfirmDialog.vue'
const posts = useResource(postApi.admin, [])
const toast = useNotificationStore()
const selected = ref(null)
const busy = ref(false)
const actionId = ref(null)
onMounted(() => posts.reload())
async function remove() {
  if (busy.value) return
  busy.value = true
  try {
    await postApi.remove(selected.value.id)
    selected.value = null
    toast.add('Yazı silindi.')
    await posts.reload()
  } catch (cause) {
    toast.add(apiError(cause), 'error')
  } finally {
    busy.value = false
  }
}
async function publish(post, value) {
  if (actionId.value !== null) return
  actionId.value = post.id
  try {
    await postApi.publish(post.id, value)
    toast.add(value ? 'Yazı yayınlandı.' : 'Yazı taslağa alındı.')
    await posts.reload()
  } catch (cause) {
    toast.add(apiError(cause), 'error')
  } finally {
    actionId.value = null
  }
}
</script>
<template>
  <div class="page-heading">
    <div>
      <span class="eyebrow">İÇERİK YÖNETİMİ</span>
      <h1>Yazılar</h1>
    </div>
    <RouterLink class="button button--primary" to="/admin/posts/new">+ Yeni Yazı</RouterLink>
  </div>
  <ResourceState :loading="posts.loading.value" :error="posts.error.value" @retry="posts.reload()"
    ><div v-if="posts.data.value.length" class="table-wrap">
      <table class="admin-table">
        <thead>
          <tr>
            <th>ID / Başlık</th>
            <th>Yazar</th>
            <th>Kategori / Tagler</th>
            <th>Tarih</th>
            <th>İşlemler</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="post in posts.data.value" :key="post.id">
            <td data-label="Başlık">
              <small>#{{ post.id }}</small
              ><strong>{{ post.title }}</strong>
            </td>
            <td data-label="Yazar">{{ post.authorName }}</td>
            <td data-label="Kategori / Tagler">
              <span>{{ post.categoryName || '—' }}</span>
              <div class="chips">
                <span v-for="tag in post.tags" :key="tag" class="tag">{{ tag }}</span>
              </div>
            </td>
            <td data-label="Tarih">{{ formatDate(post.createdAt) }}</td>
            <td data-label="İşlemler">
              <div class="table-actions">
                <RouterLink :to="`/admin/posts/${post.id}/edit`">Düzenle</RouterLink
                ><button :disabled="actionId !== null" @click="publish(post, true)">Yayınla</button
                ><button :disabled="actionId !== null" @click="publish(post, false)">
                  Taslağa Al</button
                ><button class="danger-text" :disabled="actionId !== null" @click="selected = post">
                  Sil
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <EmptyState v-else title="Henüz bir yazı yok." /></ResourceState
  ><ConfirmDialog
    :open="!!selected"
    :busy="busy"
    :message="`“${selected?.title || ''}” yazısını silmek istediğinize emin misiniz? Bu işlem geri alınamaz.`"
    @close="selected = null"
    @confirm="remove"
  />
</template>
