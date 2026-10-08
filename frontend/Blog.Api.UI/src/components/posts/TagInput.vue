<script setup>
import { computed, ref, useId } from 'vue'
const props = defineProps({
  modelValue: { type: Array, required: true },
  suggestions: { type: Array, default: () => [] },
})
const emit = defineEmits(['update:modelValue'])
const input = ref('')
const id = useId()
const matches = computed(() =>
  props.suggestions
    .filter(
      (tag) =>
        input.value.trim() &&
        tag.name.toLocaleLowerCase('tr').includes(input.value.toLocaleLowerCase('tr')) &&
        !props.modelValue.some((name) => name.toLowerCase() === tag.name.toLowerCase()),
    )
    .slice(0, 6),
)
function add(value = input.value) {
  const tags = [...props.modelValue]
  value
    .split(',')
    .map((tag) => tag.trim())
    .filter(Boolean)
    .forEach((tag) => {
      if (!tags.some((item) => item.toLowerCase() === tag.toLowerCase())) tags.push(tag)
    })
  emit('update:modelValue', tags)
  input.value = ''
}
defineExpose({ commit: () => add() })
function onKeydown(event) {
  if (!event.isComposing && (event.key === 'Enter' || event.key === ',')) {
    event.preventDefault()
    add()
  }
}
</script>
<template>
  <div class="field">
    <label :for="id">Tagler</label>
    <div class="tag-editor">
      <span v-for="(tag, index) in modelValue" :key="tag" class="tag"
        >{{ tag
        }}<button
          type="button"
          :aria-label="`${tag} etiketini kaldır`"
          @click="
            emit(
              'update:modelValue',
              modelValue.filter((_, i) => i !== index),
            )
          "
        >
          ×
        </button></span
      ><input
        :id="id"
        v-model="input"
        placeholder="Tag yazın, Enter veya virgül ile ekleyin"
        @keydown="onKeydown"
        @blur="add()"
      />
    </div>
    <small>Yeni tagler yazı kaydedildiğinde oluşturulur.</small>
    <div v-if="matches.length" class="chips">
      <button
        v-for="tag in matches"
        :key="tag.id"
        type="button"
        class="tag"
        @mousedown.prevent
        @click="add(tag.name)"
      >
        {{ tag.name }} +
      </button>
    </div>
  </div>
</template>
