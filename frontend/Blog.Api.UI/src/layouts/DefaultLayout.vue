<script setup>
import AppNavbar from '@/components/layout/AppNavbar.vue'
import AppFooter from '@/components/layout/AppFooter.vue'
</script>
<template>
  <div class="default-layout">
    <AppNavbar />
    <main id="main-content" class="default-content"><RouterView /></main>
    <AppFooter />
  </div>
</template>
