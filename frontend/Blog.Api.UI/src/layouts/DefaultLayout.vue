<script setup>
import AppNavbar from '@/components/layout/AppNavbar.vue'
import AppFooter from '@/components/layout/AppFooter.vue'
</script>
<template>
  <AppNavbar />
  <main id="main-content"><RouterView /></main>
  <AppFooter />
</template>
