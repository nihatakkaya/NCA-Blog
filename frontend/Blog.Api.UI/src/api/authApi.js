import { api } from './axios'
export const authApi = {
  login: (body) => api.post('/Auth/login', body, { skipAuth: true }),
  register: (body) => api.post('/Auth/register', body, { skipAuth: true }),
  me: () => api.get('/Auth/me'),
  logout: (refreshToken) => api.post('/Auth/logout', { refreshToken }, { skipRefresh: true }),
}
