import { getRollupConfig } from '../../.build/build-icons.mjs'
import pkg from './package.json' assert { type: "json" }

export default getRollupConfig({
  name: 'icons-vue',
  globals: {
    vue: 'vue',
    icons: '@tabler/icons'
  },
  external: ['vue', '@tabler/icons'],
  pkg
})
