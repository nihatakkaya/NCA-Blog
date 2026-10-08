<script setup>
import { onMounted, reactive, ref } from 'vue'
import { useResource } from '@/composables/useResource'
import { apiError } from '@/api/axios'
import { useNotificationStore } from '@/stores/notification'
import ResourceState from './ResourceState.vue'
import EmptyState from './EmptyState.vue'
import BaseModal from './BaseModal.vue'
import BaseInput from './BaseInput.vue'
import BaseButton from './BaseButton.vue'
import ConfirmDialog from './ConfirmDialog.vue'
const props = defineProps({
  api: { type: Object, required: true },
  title: { type: String, required: true },
  singular: { type: String, required: true },
})
const list = useResource(props.api.list, [])
const toast = useNotificationStore()
const open = ref(false)
const selected = ref(null)
const busy = ref(false)
const error = ref('')
const form = reactive({ id: null, name: '' })
onMounted(() => list.reload())
function edit(item = null) {
  form.id = item?.id ?? null
  form.name = item?.name || ''
  error.value = ''
  open.value = true
}
async function save() {
  if (busy.value || !form.name.trim()) return
  busy.value = true
  error.value = ''
  try {
    const body = { name: form.name.trim() }
    if (form.id !== null) await props.api.update(form.id, body)
    else await props.api.create(body)
    open.value = false
    toast.add(`${props.singular} ${form.id !== null ? 'güncellendi' : 'oluşturuldu'}.`)
    await list.reload()
  } catch (cause) {
    error.value = apiError(cause)
  } finally {
    busy.value = false
  }
}
async function remove() {
  if (busy.value) return
  busy.value = true
  try {
    await props.api.remove(selected.value.id)
    selected.value = null
    toast.add(`${props.singular} silindi.`)
    await list.reload()
  } catch (cause) {
    toast.add(apiError(cause), 'error')
  } finally {
    busy.value = false
  }
}
</script>
<template>
  <div class="page-heading">
    <div>
      <span class="eyebrow">İÇERİK YÖNETİMİ</span>
      <h1>{{ title }}</h1>
    </div>
    <BaseButton @click="edit()">+ Yeni {{ singular }}</BaseButton>
  </div>
  <ResourceState :loading="list.loading.value" :error="list.error.value" @retry="list.reload()"
    ><div v-if="list.data.value.length" class="panel taxonomy-list">
      <div v-for="item in list.data.value" :key="item.id" class="taxonomy-row">
        <div>
          <small class="muted">#{{ item.id }}</small
          ><strong>{{ item.name }}</strong>
        </div>
        <div class="actions">
          <button class="text-button" @click="edit(item)">Düzenle</button
          ><button class="text-button danger-text" @click="selected = item">Sil</button>
        </div>
      </div>
    </div>
    <EmptyState
      v-else
      :title="`Henüz bir ${singular.toLocaleLowerCase('tr')} yok.`" /></ResourceState
  ><BaseModal
    :open="open"
    :busy="busy"
    :title="`${singular} ${form.id !== null ? 'Düzenle' : 'Oluştur'}`"
    @close="open = false"
    ><form @submit.prevent="save">
      <BaseInput v-model="form.name" label="Ad" required :disabled="busy" />
      <p v-if="error" class="form-error" role="alert">{{ error }}</p>
      <div class="actions">
        <BaseButton type="submit" :loading="busy">Kaydet</BaseButton
        ><BaseButton variant="secondary" :disabled="busy" @click="open = false">Vazgeç</BaseButton>
      </div>
    </form></BaseModal
  ><ConfirmDialog
    :open="!!selected"
    :busy="busy"
    :message="`“${selected?.name || ''}” kaydını silmek istediğinize emin misiniz?`"
    @close="selected = null"
    @confirm="remove"
  />
</template>
