<script setup>
import { ref, watch, nextTick, onBeforeUnmount, useId } from 'vue'
const props = defineProps({ open: Boolean, title: { type: String, required: true }, busy: Boolean })
const emit = defineEmits(['close'])
const dialog = ref(null)
const titleId = useId()
let previousFocus
watch(
  () => props.open,
  async (open) => {
    await nextTick()
    if (open) {
      previousFocus = document.activeElement
      dialog.value?.showModal()
    } else {
      dialog.value?.close()
      previousFocus?.focus()
    }
  },
)
onBeforeUnmount(() => {
  dialog.value?.close()
  previousFocus?.focus()
})
</script>
<template>
  <Teleport to="body"
    ><dialog
      ref="dialog"
      class="modal"
      :aria-labelledby="titleId"
      @cancel.prevent="!busy && emit('close')"
    >
      <div class="modal-heading">
        <h2 :id="titleId">{{ title }}</h2>
        <button
          class="icon-button"
          aria-label="Pencereyi kapat"
          :disabled="busy"
          @click="emit('close')"
        >
          ×
        </button>
      </div>
      <slot /></dialog
  ></Teleport>
</template>
