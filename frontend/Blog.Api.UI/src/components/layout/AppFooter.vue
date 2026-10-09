<script setup>
import AppLogo from '@/components/common/AppLogo.vue'
import { siteConfig } from '@/config/site'
const year = new Date().getFullYear()
</script>
<template>
  <footer class="footer">
    <div class="container footer-inner">
      <div class="footer-branding">
        <RouterLink to="/" aria-label="NCA ana sayfa"><AppLogo size="small" /></RouterLink
        ><small>© {{ year }} Nihat Akkaya</small>
      </div>
      <nav aria-label="Alt menü">
        <RouterLink to="/">Ana Sayfa</RouterLink>
      </nav>
      <div class="social-links">
        <a
          v-if="siteConfig.githubUrl"
          :href="siteConfig.githubUrl"
          target="_blank"
          rel="noopener noreferrer"
          >GitHub</a
        ><a
          v-if="siteConfig.linkedinUrl"
          :href="siteConfig.linkedinUrl"
          target="_blank"
          rel="noopener noreferrer"
          >LinkedIn</a
        ><a v-if="siteConfig.email" :href="`mailto:${siteConfig.email}`">E-posta</a>
      </div>
    </div>
  </footer>
</template>
