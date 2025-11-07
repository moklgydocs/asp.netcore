<template>
  <div>
    <li>{{ data?.id }}</li>
     <li>{{ data?.title }}</li>
     <li>{{ data?.body }}</li>

     <li>{{ data2?.id }}</li>
     <li>{{ data2?.title }}</li>
     <li>{{ data2?.body }}</li>
  </div>
</template>
<script lang="ts" setup>

interface Post {
  id: number,
  userId: number,
  title: string,
  body: string
}
import axios from 'axios'
import { onMounted, ref } from 'vue'
const BASE_URL ='https://jsonplaceholder.typicode.com';
const data = ref<Post>();
const data2 = ref<Post>();
const fetchData = async () => {
  try {
     // 发送 GET 请求到测试 API
     const response = await axios.get('https://jsonplaceholder.typicode.com/posts/1')

     console.log('✅ 请求成功！')
     console.log('响应数据：', response.data)
     console.log('响应状态：', response.status)
     console.log('响应状态文本：', response.statusText)

     return response.data
   } catch (error) {
     console.error('❌ 请求失败：', error)
     throw error
   }
}

const run1 = ()=>{
   fetchData().then((res)=>{
    data.value = res

    console.log(res)
  }).catch((error)=>{
    console.error('💥 学习过程中遇到错误：', error.message);
  });
}

const postId = ref(2);
console.log(`${BASE_URL}/posts/${postId.value}`);
const getRun1 =async ()=>{
  await axios.get(`${BASE_URL}/posts/${postId.value}`,{
    params:{
      postId: postId.value,
      _limit:5
    }
  }).then(res=>{
     data2.value = res.data
    console.log('data:',res.data);
    console.log('getrun1: ',res);
  }).catch(err=>{
    console.log(err.message);
  });
}

onMounted(async() => {
run1()
getRun1()
})
</script>
<style lang="scss">

</style>
