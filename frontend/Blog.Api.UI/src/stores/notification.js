import { defineStore } from 'pinia'
import { ref } from 'vue'
export const useNotificationStore = defineStore('notification', () => {
  const items = ref([])
  let sequence = 0
  function remove(id) {
    items.value = items.value.filter((item) => item.id !== id)
  }
  function add(message, type = 'success') {
    const id = ++sequence
    items.value.push({ id, message, type })
    setTimeout(() => remove(id), 6000)
  }
  return { items, add, remove }
})
