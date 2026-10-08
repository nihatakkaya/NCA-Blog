<script setup>
import { useId } from 'vue'
defineOptions({ inheritAttrs: false })
defineProps({ modelValue: { type: String, default: '' }, label: { type: String, required: true } })
defineEmits(['update:modelValue'])
const id = useId()
</script>
<template>
  <div class="field">
    <label :for="id">{{ label }}</label
    ><input
      :id="id"
      v-bind="$attrs"
      :value="modelValue"
      @input="$emit('update:modelValue', $event.target.value)"
    />
  </div>
</template>
