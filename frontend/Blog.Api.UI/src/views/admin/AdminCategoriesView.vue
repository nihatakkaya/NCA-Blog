<script setup>
import TaxonomyManager from '@/components/common/TaxonomyManager.vue'
import { categoryApi } from '@/api/categoryApi'
</script>
<template><TaxonomyManager :api="categoryApi" title="Kategoriler" singular="Kategori" /></template>
