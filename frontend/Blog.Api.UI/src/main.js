import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import '@fontsource/inter/latin-400.css'
import '@fontsource/inter/latin-500.css'
import '@fontsource/inter/latin-600.css'
import '@fontsource/inter/latin-700.css'
import '@fontsource/inter/latin-ext-400.css'
import '@fontsource/inter/latin-ext-500.css'
import '@fontsource/inter/latin-ext-600.css'
import '@fontsource/inter/latin-ext-700.css'
import './assets/styles/tokens.css'
import './assets/styles/base.css'
import './assets/styles/utilities.css'

const app = createApp(App)

app.use(createPinia())
app.use(router)

window.addEventListener('nca:expired', () => {
  if (router.currentRoute.value.path !== '/login')
    router.replace({ path: '/login', query: { redirect: router.currentRoute.value.fullPath } })
})

app.mount('#app')
