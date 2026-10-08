import { ref } from 'vue'
import { apiError } from '@/api/axios'
export function useResource(loader, initial = null) {
  const data = ref(initial)
  const loading = ref(false)
  const error = ref('')
  let sequence = 0
  async function reload(...args) {
    const request = ++sequence
    loading.value = true
    error.value = ''
    try {
      const result = await loader(...args)
      if (request === sequence) data.value = result.data
    } catch (cause) {
      if (request === sequence) error.value = apiError(cause)
    } finally {
      if (request === sequence) loading.value = false
    }
  }
  return { data, loading, error, reload }
}
