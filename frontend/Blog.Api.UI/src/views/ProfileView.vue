<script setup>
import { onMounted } from 'vue'
import { authApi } from '@/api/authApi'
import { useResource } from '@/composables/useResource'
import ResourceState from '@/components/common/ResourceState.vue'
const profile = useResource(authApi.me)
onMounted(() => profile.reload())
</script>
<template>
  <div class="reading-container">
    <span class="eyebrow">HESABINIZ</span>
    <h1>Profil</h1>
    <ResourceState
      :loading="profile.loading.value"
      :error="profile.error.value"
      @retry="profile.reload()"
      ><div v-if="profile.data.value" class="panel">
        <dl class="profile-list">
          <dt>Kullanıcı adı</dt>
          <dd>{{ profile.data.value.username }}</dd>
          <dt>E-posta</dt>
          <dd>{{ profile.data.value.email }}</dd>
          <dt>Rol</dt>
          <dd>{{ profile.data.value.role }}</dd>
        </dl>
        <RouterLink
          v-if="profile.data.value.role === 'Admin'"
          class="button button--primary"
          to="/admin"
          >Yönetim paneline geç</RouterLink
        >
      </div></ResourceState
    >
  </div>
</template>
