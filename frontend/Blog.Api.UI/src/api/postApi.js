import { api } from './axios'
export const postApi = {
  list: (page = 1, pageSize = 9) =>
    api.get('/Post', { params: { page, pageSize }, skipAuth: true }),
  search: (query) => api.get('/Post/search', { params: { query }, skipAuth: true }),
  detail: (id) => api.get(`/Post/${id}`, { skipAuth: true }),
  admin: () => api.get('/Post/admin'),
  create: (body) => api.post('/Post', body),
  update: (id, body) => api.put(`/Post/${id}`, body),
  remove: (id) => api.delete(`/Post/${id}`),
  publish: (id, isPublished) => api.put(`/Post/${id}/publish`, null, { params: { isPublished } }),
}
