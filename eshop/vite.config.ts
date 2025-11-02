import { fileURLToPath, URL } from "node:url";

import { defineConfig } from "vite";
import AutoImport from "unplugin-auto-import/vite";
import Components from "unplugin-vue-components/vite";
import { ElementPlusResolver } from "unplugin-vue-components/resolvers";
import ElementPlus from "unplugin-element-plus/vite"; // 引入样式按需加载插件

import vue from "@vitejs/plugin-vue";
import vueDevTools from "vite-plugin-vue-devtools";

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(), 
    AutoImport({
      resolvers: [ElementPlusResolver()], // 自动导入 Element Plus 的 API
    }),
    Components({
      resolvers: [
        ElementPlusResolver(), // 自动导入 Element Plus 的组件
      ],
    }),
    ElementPlus({
      // 按需加载 Element Plus 组件的样式
      useSource: true, // 使用源代码
    }),
  ],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  }, 
});
