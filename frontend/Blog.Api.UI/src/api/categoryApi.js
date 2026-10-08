import { api } from './axios'
export const categoryApi = {
  list: () => api.get('/Category', { skipAuth: true }),
  create: (body) => api.post('/Category', body),
  update: (id, body) => api.put(`/Category/${id}`, body),
  remove: (id) => api.delete(`/Category/${id}`),
}
