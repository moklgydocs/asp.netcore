<template>


     <!-- <ElButton>Click me</ElButton>
  <el-button type="primary">Primary Button</el-button>
  <el-button type="success">Success Button</el-button>
  <el-button type="info">Info Button</el-button>
  <el-button type="warning">Warning Button</el-button> -->


  <AxiosDemo></AxiosDemo>

</template>


<script setup lang="ts">
// import HelloWorld from '@/components/HelloWorld.vue'
// import TheWelcome from '@/components/TheWelcome.vue'
import { useCounterStore } from '@/stores/counter'
// import { ElButton } from 'element-plus';
// import AxiosLearning from '@/components/AxiosLearning.vue'

import AxiosDemo from '@/views/axios-demo.vue'

const counter = useCounterStore()
counter.count++

</script>

<style scoped>

</style>
