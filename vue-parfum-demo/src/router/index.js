import { createRouter, createWebHistory } from 'vue-router'
import home from '@/views/MainWindow.vue'
import LoginPage from '@/views/Auth.vue'
import Register from '@/views/Register.vue'
import Cart from '@/views/Cart.vue'
import Orders from '@/views/Orders.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'Login',
      component: LoginPage,
    },
    {
      path: '/home',
      name: 'home',
      component: home
    },
    {
    path: '/product/add',
    name: 'AddProduct',
    component: () => import('../views/ProductForm.vue')
  },
  {
    path: '/product/edit/:id',
    name: 'EditProduct',
    component: () => import('../views/ProductForm.vue')
  },
  {
    path: '/register',
    name: 'register',
    component: Register
  },
  {
    path: '/cart',
    name: 'cart',
    component: Cart
  },
  {
    path: '/orders',
    name: 'orders',
    component: Orders
  }
  ],
})

export default router
