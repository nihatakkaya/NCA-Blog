import { api } from './axios'
const path = (postId) => `/posts/${postId}/comments`
export const commentApi = {
  list: (postId) => api.get(path(postId), { skipAuth: true }),
  create: (postId, body) => api.post(path(postId), body),
  update: (postId, id, body) => api.put(`${path(postId)}/${id}`, body),
  remove: (postId, id) => api.delete(`${path(postId)}/${id}`),
}
