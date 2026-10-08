<script setup>
import BaseModal from './BaseModal.vue'
import BaseButton from './BaseButton.vue'
defineProps({
  open: Boolean,
  busy: Boolean,
  title: { type: String, default: 'Kaydı sil' },
  message: {
    type: String,
    default: 'Bu kaydı silmek istediğinize emin misiniz? Bu işlem geri alınamaz.',
  },
})
defineEmits(['close', 'confirm'])
</script>
<template>
  <BaseModal :open="open" :title="title" :busy="busy" @close="$emit('close')"
    ><p>{{ message }}</p>
    <div class="actions">
      <BaseButton variant="secondary" :disabled="busy" @click="$emit('close')">Vazgeç</BaseButton
      ><BaseButton variant="danger" :loading="busy" @click="$emit('confirm')">Sil</BaseButton>
    </div></BaseModal
  >
</template>
