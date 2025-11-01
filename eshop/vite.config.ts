import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
//import plugin from "@vitejs/plugin-vue";//这个插件就不需要了会报错
import vueDevTools from "vite-plugin-vue-devtools";
import path from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue(),  vueDevTools()],
  server: {
    port: 59883,
  },
  resolve: {
    alias: {
      // 配置“@”指向src目录（与tsconfig保持一致）
      '@': path.resolve(__dirname, './src')
    }
  }
});
