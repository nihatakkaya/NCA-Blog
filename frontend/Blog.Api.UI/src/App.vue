<script setup>
import AppToast from '@/components/common/AppToast.vue'
import { useAuthStore } from '@/stores/auth'
const auth = useAuthStore()
</script>

<template>
  <a class="skip-link" href="#main-content">İçeriğe geç</a>
  <div v-if="auth.restoreError" class="session-banner" role="alert">{{ auth.restoreError }}</div>
  <RouterView />
  <AppToast />
</template>

<style scoped></style>
