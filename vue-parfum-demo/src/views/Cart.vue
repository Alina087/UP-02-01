<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { useCookies } from 'vue3-cookies'

const router = useRouter()
const { cookies } = useCookies()

const cartItems = ref([])
const isLoading = ref(false)
const error = ref(null)
const userData = ref(null)
const pickUpPoints = ref([])
const selectedPickUpPoint = ref(null)

const fetchPickUpPoints = async () => {
  try {
    const response = await axios.get('http://localhost:5122/api/PickUpPoint/GetAll')
    pickUpPoints.value = response.data
    if (pickUpPoints.value.length > 0) {
      selectedPickUpPoint.value = pickUpPoints.value[0].pickUpPointId
    }
  } catch (error) {
    console.error('Ошибка при загрузке пунктов выдачи:', error)
  }
}

const loadCart = async () => {
  if (!userData.value) return
  
  try {
    isLoading.value = true
    error.value = null
    
    const response = await axios.get(`http://localhost:5122/api/Cart/GetCart/${userData.value.userId}`)
    
    if (response.data && Array.isArray(response.data)) {
      cartItems.value = response.data
    } else {
      cartItems.value = []
    }
    
  } catch (err) {
    console.error('Ошибка при загрузке корзины:', err)
    error.value = 'Ошибка при загрузке корзины'
    cartItems.value = []
  } finally {
    isLoading.value = false
  }
}

const removeFromCart = async (article) => {
  if (!userData.value) return
  
  try {
    await axios.delete(`http://localhost:5122/api/Cart/RemoveFromCart`, {
      params: {
        userId: userData.value.userId,
        tovarArticle: article
      }
    })
    
    await loadCart()
    window.dispatchEvent(new Event('storage'))
    
  } catch (err) {
    console.error('Ошибка при удалении из корзины:', err)
    alert('Ошибка при удалении товара')
  }
}

const updateQuantity = async (item, newQuantity) => {
  if (!userData.value) return
  
  if (newQuantity < 1) {
    await removeFromCart(item.tovarArticle)
    return
  }
  
  try {
    await axios.put(`http://localhost:5122/api/Cart/UpdateCartItem`, null, {
      params: {
        userId: userData.value.userId,
        tovarArticle: item.tovarArticle,
        quantity: newQuantity
      }
    })
    
    item.cartTovarCount = newQuantity
    window.dispatchEvent(new Event('storage'))
    
  } catch (err) {
    console.error('Ошибка при обновлении количества:', err)
    alert('Ошибка при обновлении количества')
    await loadCart()
  }
}

const clearCart = async () => {
  if (!userData.value) return
  
  if (!confirm('Очистить корзину?')) return
  
  try {
    await axios.delete(`http://localhost:5122/api/Cart/ClearCart/${userData.value.userId}`)
    cartItems.value = []
    window.dispatchEvent(new Event('storage'))
    
  } catch (err) {
    console.error('Ошибка при очистке корзины:', err)
    alert('Ошибка при очистке корзины')
  }
}

const getDiscountedPrice = (item) => {
  const cost = item.tovarCost || 0
  const discount = item.tovarDiscount || 0
  return cost * (1 - discount / 100)
}

const totalCost = computed(() => {
  return cartItems.value.reduce((sum, item) => {
    const price = getDiscountedPrice(item)
    return sum + (price * (item.cartTovarCount || 0))
  }, 0)
})

const totalItems = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.cartTovarCount || 0), 0)
})

const getImagePath = (item) => {
  if (item.tovarImage) {
    return `/resources/${item.tovarImage}`
  }
  return '/resources/picture.png'
}

const checkout = async () => {
  if (!userData.value) {
    alert('Необходимо авторизоваться для оформления заказа')
    router.push('/login')
    return
  }
  
  if (cartItems.value.length === 0) {
    alert('Корзина пуста')
    return
  }

  if (!selectedPickUpPoint.value) {
    alert('Выберите пункт выдачи')
    return
  }
  
  try {
    isLoading.value = true
    
    await axios.post('http://localhost:5122/api/Order/CreateOrder', null, {
      params: {
        userId: userData.value.userId,
        pickUpPointId: selectedPickUpPoint.value
      }
    })
    
    alert('Заказ успешно оформлен!')
    cartItems.value = []
    window.dispatchEvent(new Event('storage'))
    router.push('/orders')
    
  } catch (err) {
    console.error('Ошибка при оформлении заказа:', err)
    if (err.response) {
      alert(err.response.data || 'Ошибка при оформлении заказа')
    } else {
      alert('Ошибка при оформлении заказа')
    }
  } finally {
    isLoading.value = false
  }
}

const goToOrders = () => {
  router.push('/orders')
}

onMounted(() => {
  userData.value = cookies.get('user')
  if (userData.value) {
    loadCart()
    fetchPickUpPoints()
  }
})
</script>

<template>
  <div class="cart-wrapper">
    <div class="cart-container">
      <div class="cart-header">
        <h1>Корзина</h1>
        <div class="header-actions">
          <button @click="goToOrders" class="btn-orders">
            📋 Мои заказы
          </button>
          <button v-if="cartItems.length > 0" @click="clearCart" class="btn-clear" :disabled="isLoading">
            Очистить корзину
          </button>
        </div>
      </div>
      
      <div v-if="isLoading" class="loading">
        <div class="spinner"></div>
        <p>Загрузка корзины...</p>
      </div>
      
      <div v-else-if="!userData" class="empty-cart">
        <div class="empty-cart-icon">🔒</div>
        <h2>Требуется авторизация</h2>
        <p>Войдите в аккаунт, чтобы просмотреть корзину</p>
        <button @click="router.push('/')" class="btn-shop">
          Войти
        </button>
      </div>
      
      <div v-else-if="cartItems.length === 0" class="empty-cart">
        <div class="empty-cart-icon">🛒</div>
        <h2>Корзина пуста</h2>
        <p>Добавьте товары в корзину, чтобы оформить заказ</p>
        <button @click="router.push('/home')" class="btn-shop">
          Перейти к покупкам
        </button>
      </div>
      
      <div v-else class="cart-content">
        <div class="cart-items">
          <div v-for="item in cartItems" :key="item.tovarArticle" class="cart-item">
            <div class="item-image">
              <img 
                :src="getImagePath(item)" 
                :alt="item.tovarName"
                @error="(e) => { e.target.src = '/resources/picture.png' }"
              >
            </div>
            
            <div class="item-info">
              <h3 class="item-name">{{ item.tovarName || 'Товар не найден' }}</h3>
              <p class="item-article">Артикул: {{ item.tovarArticle }}</p>
              <p class="item-unit">Ед. измерения: {{ item.tovarUnit || 'шт.' }}</p>
              <p v-if="item.manufacturerName" class="item-manufacturer">
                Производитель: {{ item.manufacturerName }}
              </p>
              <p v-if="item.tovarDiscount > 0" class="item-discount">
                Скидка: {{ item.tovarDiscount }}%
              </p>
            </div>
            
            <div class="item-price">
              <div class="price-current">
                {{ (getDiscountedPrice(item) * (item.cartTovarCount || 0)).toFixed(2) }} ₽
              </div>
              <div v-if="item.tovarDiscount > 0" class="price-old">
                {{ ((item.tovarCost || 0) * (item.cartTovarCount || 0)).toFixed(2) }} ₽
              </div>
            </div>
            
            <div class="item-quantity">
              <button 
                @click="updateQuantity(item, (item.cartTovarCount || 0) - 1)"
                class="quantity-btn"
                :disabled="isLoading"
              >-</button>
              <input 
                type="number" 
                :value="item.cartTovarCount"
                @change="updateQuantity(item, parseInt($event.target.value))"
                min="1"
                :max="item.tovarCount"
                class="quantity-input"
                :disabled="isLoading"
              >
              <button 
                @click="updateQuantity(item, (item.cartTovarCount || 0) + 1)"
                class="quantity-btn"
                :disabled="isLoading || (item.cartTovarCount || 0) >= (item.tovarCount || 0)"
              >+</button>
            </div>
            
            <button 
              @click="removeFromCart(item.tovarArticle)"
              class="btn-remove"
              :disabled="isLoading"
              title="Удалить из корзины"
            >
              ×
            </button>
          </div>
        </div>
        
        <div class="cart-summary">
          <h2>Итого</h2>
          <div class="summary-row">
            <span>Товаров:</span>
            <span>{{ totalItems }} шт.</span>
          </div>
          <div class="summary-row total">
            <span>Сумма:</span>
            <span>{{ totalCost.toFixed(2) }} ₽</span>
          </div>
          
          <div class="pickup-section">
            <label class="pickup-label">Пункт выдачи:</label>
            <select v-model="selectedPickUpPoint" class="pickup-select">
              <option v-for="point in pickUpPoints" :key="point.pickUpPointId" :value="point.pickUpPointId">
                {{ point.pickUpPointAdress }}
              </option>
            </select>
          </div>
          
          <button 
            @click="checkout" 
            class="btn-checkout"
            :disabled="isLoading"
          >
            Оформить заказ
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.cart-wrapper {
  min-height: 100vh;
  background: white;
  padding: 80px 20px 40px;
}

.cart-container {
  max-width: 1200px;
  margin: 0 auto;
  background: white;
  border-radius: 16px;
  box-shadow: 0 8px 30px rgba(255, 126, 179, 0.1);
  overflow: hidden;
}

.cart-header {
  padding: 30px 40px;
  background: linear-gradient(135deg, #ff4081 0%, #ff7eb3 100%);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.cart-header h1 {
  font-size: 24px;
  font-weight: 600;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.btn-orders {
  padding: 8px 16px;
  background: #4CAF50;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-orders:hover:not(:disabled) {
  background: #45a049;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(76, 175, 80, 0.3);
}

.btn-clear {
  padding: 8px 16px;
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.4);
  border-radius: 8px;
  color: white;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-clear:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

.empty-cart {
  padding: 80px 40px;
  text-align: center;
}

.empty-cart-icon {
  font-size: 80px;
  margin-bottom: 20px;
  opacity: 0.5;
}

.empty-cart h2 {
  color: #333;
  margin-bottom: 10px;
}

.empty-cart p {
  color: #666;
  margin-bottom: 30px;
}

.btn-shop {
  padding: 12px 30px;
  background: linear-gradient(135deg, #ff4081 0%, #ff7eb3 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-shop:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 64, 129, 0.3);
}

.cart-content {
  display: grid;
  grid-template-columns: 1fr 350px;
  gap: 30px;
  padding: 40px;
}

@media (max-width: 992px) {
  .cart-content {
    grid-template-columns: 1fr;
  }
}

.cart-items {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.cart-item {
  display: grid;
  grid-template-columns: 100px 1fr auto auto auto;
  gap: 20px;
  align-items: center;
  padding: 20px;
  background: #f9f9f9;
  border-radius: 12px;
  transition: all 0.3s ease;
}

.cart-item:hover {
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
}

@media (max-width: 768px) {
  .cart-item {
    grid-template-columns: 80px 1fr;
    grid-template-rows: auto auto auto;
    gap: 15px;
  }
  
  .item-price {
    grid-column: 2;
    grid-row: 2;
  }
  
  .item-quantity {
    grid-column: 2;
    grid-row: 3;
  }
  
  .btn-remove {
    grid-column: 1;
    grid-row: 1;
    justify-self: start;
  }
}

.item-image {
  width: 100px;
  height: 100px;
  border-radius: 8px;
  overflow: hidden;
  border: 2px solid #e0e0e0;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

@media (max-width: 768px) {
  .item-image {
    width: 80px;
    height: 80px;
  }
}

.item-info h3 {
  margin: 0 0 5px 0;
  font-size: 16px;
  color: #333;
}

.item-article, .item-unit, .item-manufacturer, .item-discount {
  margin: 2px 0;
  font-size: 13px;
  color: #666;
}

.item-discount {
  color: #4CAF50;
}

.item-manufacturer {
  color: #ff7eb3;
}

.item-price {
  text-align: right;
}

.price-current {
  font-size: 18px;
  font-weight: 600;
  color: #ff4081;
}

.price-old {
  font-size: 14px;
  color: #999;
  text-decoration: line-through;
}

.item-quantity {
  display: flex;
  align-items: center;
  gap: 5px;
}

.quantity-btn {
  width: 30px;
  height: 30px;
  background: white;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  cursor: pointer;
  font-size: 16px;
  font-weight: 600;
  transition: all 0.3s ease;
}

.quantity-btn:hover:not(:disabled) {
  border-color: #ff4081;
  color: #ff4081;
}

.quantity-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity-input {
  width: 50px;
  height: 30px;
  text-align: center;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  font-size: 14px;
}

.quantity-input:focus {
  outline: none;
  border-color: #ff4081;
}

.btn-remove {
  width: 30px;
  height: 30px;
  background: rgba(255, 64, 129, 0.1);
  color: #ff4081;
  border: none;
  border-radius: 50%;
  font-size: 20px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-remove:hover:not(:disabled) {
  background: #ff4081;
  color: white;
  transform: scale(1.1);
}

.btn-remove:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.cart-summary {
  padding: 25px;
  background: #f9f9f9;
  border-radius: 12px;
  position: sticky;
  top: 100px;
}

.cart-summary h2 {
  margin: 0 0 20px 0;
  font-size: 20px;
  color: #333;
  text-align: center;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 15px;
  color: #666;
  font-size: 15px;
}

.summary-row.total {
  margin-top: 20px;
  padding-top: 20px;
  border-top: 2px solid #e0e0e0;
  font-size: 20px;
  font-weight: 600;
  color: #333;
}

.pickup-section {
  margin: 20px 0;
  padding: 15px 0;
  border-top: 1px solid #e0e0e0;
  border-bottom: 1px solid #e0e0e0;
}

.pickup-label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #ff4081;
  font-size: 14px;
}

.pickup-select {
  width: 100%;
  padding: 12px;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-size: 14px;
  background: white;
  cursor: pointer;
  transition: all 0.3s ease;
}

.pickup-select:focus {
  outline: none;
  border-color: #ff4081;
  box-shadow: 0 0 0 3px rgba(255, 64, 129, 0.1);
}

.btn-checkout {
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-checkout:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(76, 175, 80, 0.3);
}

.btn-checkout:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  background: #999;
}

.loading {
  padding: 60px 40px;
  text-align: center;
  color: #666;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 15px;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #ff4081;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>