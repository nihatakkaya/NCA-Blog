import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import DefaultLayout from '@/layouts/DefaultLayout.vue'
import AdminLayout from '@/layouts/AdminLayout.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  scrollBehavior(to) {
    return to.hash ? { el: to.hash, top: 100, behavior: 'instant' } : { top: 0 }
  },
  routes: [
    {
      path: '/',
      component: DefaultLayout,
      children: [
        { path: '', component: () => import('@/views/HomeView.vue') },
        { path: 'login', component: () => import('@/views/LoginView.vue') },
        { path: 'register', component: () => import('@/views/RegisterView.vue') },
        {
          path: 'profile',
          meta: { requiresAuth: true },
          component: () => import('@/views/ProfileView.vue'),
        },
        { path: 'post/:id', component: () => import('@/views/PostDetailView.vue') },
        { path: ':pathMatch(.*)*', component: () => import('@/views/NotFoundView.vue') },
      ],
    },
    {
      path: '/admin',
      component: AdminLayout,
      meta: { requiresAuth: true, requiresAdmin: true },
      children: [
        { path: '', component: () => import('@/views/admin/AdminDashboardView.vue') },
        { path: 'posts', component: () => import('@/views/admin/AdminPostsView.vue') },
        { path: 'posts/new', component: () => import('@/views/admin/AdminPostFormView.vue') },
        { path: 'posts/:id/edit', component: () => import('@/views/admin/AdminPostFormView.vue') },
        { path: 'categories', component: () => import('@/views/admin/AdminCategoriesView.vue') },
        { path: 'tags', component: () => import('@/views/admin/AdminTagsView.vue') },
      ],
    },
  ],
})

router.beforeEach(async (to) => {
  const auth = useAuthStore()
  await auth.restore()
  if (to.meta.requiresAuth && !auth.isAuthenticated)
    return { path: '/login', query: { redirect: to.fullPath } }
  if (to.meta.requiresAdmin && !auth.isAdmin) return '/'
})

export default router
