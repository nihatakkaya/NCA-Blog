import { api } from './axios'
export const tagApi = {
  list: () => api.get('/Tag', { skipAuth: true }),
  create: (body) => api.post('/Tag', body),
  update: (id, body) => api.put(`/Tag/${id}`, body),
  remove: (id) => api.delete(`/Tag/${id}`),
}
