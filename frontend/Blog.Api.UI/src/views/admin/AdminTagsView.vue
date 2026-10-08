<script setup>
import TaxonomyManager from '@/components/common/TaxonomyManager.vue'
import { tagApi } from '@/api/tagApi'
</script>
<template><TaxonomyManager :api="tagApi" title="Tagler" singular="Tag" /></template>
