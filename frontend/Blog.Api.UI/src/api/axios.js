import axios from 'axios'
import { session } from './session'

const options = { baseURL: import.meta.env.VITE_API_BASE_URL, timeout: 15000 }
export const api = axios.create(options)
const refreshClient = axios.create(options)
let refreshPromise
api.interceptors.request.use((config) => {
  if (session.access && !config.skipAuth) config.headers.Authorization = `Bearer ${session.access}`
  return config
})
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const config = error.config
    if (
      !config ||
      config.skipAuth ||
      config.skipRefresh ||
      error.response?.status !== 401 ||
      /\/Auth\/(login|register|refresh|logout)$/.test(config.url)
    )
      throw error
    if (config._retried || !session.refresh) {
      session.clear()
      window.dispatchEvent(new Event('nca:expired'))
      throw error
    }
    config._retried = true
    try {
      // All concurrent 401 responses share one refresh to respect token rotation.
      if (!refreshPromise) {
        refreshPromise = refreshClient
          .post('/Auth/refresh', { refreshToken: session.refresh })
          .then(({ data }) => {
            session.save(data)
          })
          .finally(() => {
            refreshPromise = undefined
          })
      }
      await refreshPromise
    } catch (refreshError) {
      session.clear()
      window.dispatchEvent(new Event('nca:expired'))
      throw refreshError
    }
    config.headers.Authorization = `Bearer ${session.access}`
    return api(config)
  },
)

export function apiError(error) {
  const status = error.response?.status
  if (!error.response)
    return 'API’ye ulaşılamıyor. Bağlantınızı ve sunucunun çalıştığını kontrol edip tekrar deneyin.'
  if (status >= 500) return 'Sunucuda bir sorun oluştu. Lütfen daha sonra tekrar deneyin.'
  if (status === 403) return 'Bu işlem için yetkiniz bulunmuyor.'
  const data = error.response.data
  if (data?.errors && typeof data.errors === 'object')
    return (
      Object.values(data.errors)
        .flat()
        .filter((message) => typeof message === 'string')
        .join(' ') || 'Alanları kontrol edin.'
    )
  if (typeof data?.message === 'string') return data.message
  return (
    {
      400: 'Gönderilen bilgileri kontrol edin.',
      401: 'Oturumunuz geçersiz. Lütfen giriş yapın.',
      404: 'İstenen kayıt bulunamadı.',
    }[status] || 'İşlem tamamlanamadı. Lütfen tekrar deneyin.'
  )
}
