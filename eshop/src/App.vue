<template>
  <header>
    <img alt="Vue logo" class="logo" src="./assets/logo.svg" width="125" height="125" />

    <div class="wrapper">
      <HelloWorld msg="You did it!" />
    </div>

     <ElButton>Click me</ElButton>
  <el-button type="primary">Primary Button</el-button>
  <el-button type="success">Success Button</el-button>
  <el-button type="info">Info Button</el-button>
  <el-button type="warning">Warning Button</el-button>
  </header>

  <main>
    <TheWelcome />
  </main>
</template>


<script setup lang="ts">
import HelloWorld from '@/components/HelloWorld.vue'
import TheWelcome from '@/components/TheWelcome.vue'
import { useCounterStore } from '@/stores/counter'
import { ElButton } from 'element-plus';


const counter = useCounterStore()
counter.count++

</script>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>
