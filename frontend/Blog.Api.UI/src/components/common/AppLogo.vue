<script setup>
defineProps({ size: { type: String, default: 'default' }, decorative: Boolean })
</script>
<template>
  <span class="app-logo" :class="'app-logo--' + size">
    <img src="/nca-logo.png" :alt="decorative ? '' : 'NCA'" width="740" height="740" />
  </span>
</template>
<style scoped>
.app-logo {
  position: relative;
  display: block;
  width: 100px;
  aspect-ratio: 2.5;
  overflow: hidden;
  flex-shrink: 0;
}
.app-logo img {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 135%;
  max-width: none;
  height: auto;
  transform: translate(-50%, -50%);
}
.app-logo--footer {
  width: 128px;
}
.app-logo--small {
  width: 86px;
}
.app-logo--auth {
  width: 120px;
}
.app-logo--hero {
  width: 210px;
}
@media (max-width: 600px) {
  .app-logo--footer {
    width: 104px;
  }
  .app-logo--hero {
    width: 82px;
  }
}
</style>
