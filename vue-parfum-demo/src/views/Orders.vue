<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { useCookies } from 'vue3-cookies'

const router = useRouter()
const { cookies } = useCookies()

const orders = ref([])
const filteredOrders = ref([])
const isLoading = ref(false)
const userData = ref(null)
const userRole = ref('')
const selectedStatus = ref('all')
const error = ref(null)

const fetchOrders = async () => {
  if (!userData.value) return
  
  try {
    isLoading.value = true
    error.value = null
    
    let response
    if (userRole.value === 'Администратор' || userRole.value === 'Менеджер') {
      console.log('Загрузка всех заказов для админа/менеджера')
      response = await axios.get('http://localhost:5122/api/Order/GetAllOrders')
    } else {
      console.log('Загрузка заказов пользователя:', userData.value.userId)
      response = await axios.get(`http://localhost:5122/api/Order/GetUserOrders/${userData.value.userId}`)
    }
    
    console.log('Получены заказы:', response.data)
    
    if (response.data && Array.isArray(response.data)) {
      orders.value = response.data
    } else if (response.data && response.data.$values) {
      orders.value = response.data.$values
    } else {
      console.error('Неожиданный формат данных:', response.data)
      orders.value = []
    }
    
    filterOrders()
    
  } catch (err) {
    console.error('Ошибка при загрузке заказов:', err)
    error.value = 'Ошибка при загрузке заказов'
    if (err.response) {
      console.error('Детали ошибки:', err.response.data)
    }
  } finally {
    isLoading.value = false
  }
}

const filterOrders = () => {
  if (selectedStatus.value === 'all') {
    filteredOrders.value = orders.value
  } else {
    filteredOrders.value = orders.value.filter(order => order.orderStatus === selectedStatus.value)
  }
}

watch(selectedStatus, () => {
  filterOrders()
})

const completeOrder = async (orderId) => {
  if (userRole.value !== 'Администратор') {
    alert('У вас нет прав для завершения заказа')
    return
  }
  
  const order = orders.value.find(o => o.orderId === orderId)
  if (!order) return
  
  if (order.orderStatus !== 'Новый') {
    alert('Можно завершить только новый заказ')
    return
  }
  
  if (!confirm('Завершить заказ?')) return
  
  try {
    await axios.put(`http://localhost:5122/api/Order/UpdateOrderStatus`, null, {
      params: {
        orderId: orderId,
        status: 'Завершен'
      }
    })
    
    order.orderStatus = 'Завершен'
    filterOrders()
    
    alert('Заказ успешно завершен')
    
  } catch (err) {
    console.error('Ошибка при завершении заказа:', err)
    alert('Ошибка при завершении заказа')
    await fetchOrders()
  }
}

const cancelOrder = async (orderId) => {
  if (!confirm('Вы уверены, что хотите отменить заказ?')) return
  
  try {
    await axios.delete(`http://localhost:5122/api/Order/CancelOrder/${orderId}`)
    
    orders.value = orders.value.filter(o => o.orderId !== orderId)
    filterOrders()
    
    alert('Заказ отменен')
    
  } catch (err) {
    console.error('Ошибка при отмене заказа:', err)
    if (err.response) {
      alert(err.response.data || 'Ошибка при отмене заказа')
    } else {
      alert('Ошибка при отмене заказа')
    }
  }
}

const formatDate = (dateObj) => {
  if (!dateObj) return 'Не указана'
  
  try {
    if (typeof dateObj === 'object') {
      const year = dateObj.year || dateObj.year
      const month = dateObj.month || dateObj.month
      const day = dateObj.day || dateObj.day
      if (year && month && day) {
        return `${String(day).padStart(2, '0')}.${String(month).padStart(2, '0')}.${year}`
      }
    }
    
    if (typeof dateObj === 'string') {
      const date = new Date(dateObj)
      if (!isNaN(date.getTime())) {
        return date.toLocaleDateString('ru-RU', {
          day: '2-digit',
          month: '2-digit',
          year: 'numeric'
        })
      }
    }
    
    return String(dateObj)
  } catch (e) {
    console.error('Ошибка форматирования даты:', e)
    return 'Не указана'
  }
}

const getDiscountedPrice = (cost, discount) => {
  const numCost = parseFloat(cost) || 0
  const numDiscount = parseFloat(discount) || 0
  return numCost * (1 - numDiscount / 100)
}

const getOrderTotal = (order) => {
  if (!order.structureOrders) return 0
  
  return order.structureOrders.reduce((sum, item) => {
    const tovar = item.tovar || {}
    const cost = parseFloat(tovar.tovarCost) || 0
    const discount = parseFloat(tovar.tovarDiscount) || 0
    const finalPrice = getDiscountedPrice(cost, discount)
    const count = parseInt(item.structureOrderTovarCount) || 0
    return sum + (finalPrice * count)
  }, 0)
}

const getPickUpPointAddress = (order) => {
  
  
  const parts = [
    order.pickUpPoint.pickUpPointIndex,
    order.pickUpPoint.pickUpPointCity,
    order.pickUpPoint.pickUpPointStreet,
    order.pickUpPoint.pickUpPointHome
  ]
  
  return parts.join(', ')
}

const canCompleteOrder = computed(() => {
  return userRole.value === 'Администратор'
})

const canViewAllOrders = computed(() => {
  return userRole.value === 'Администратор' || userRole.value === 'Менеджер'
})

const newOrdersCount = computed(() => {
  return orders.value.filter(o => o.orderStatus === 'Новый').length
})

const completedOrdersCount = computed(() => {
  return orders.value.filter(o => o.orderStatus === 'Завершен').length
})

onMounted(async () => {
  userData.value = cookies.get('user')
  
  if (!userData.value) {
    alert('Необходимо авторизоваться')
    router.push('/login')
    return
  }
  
  userRole.value = userData.value.userRole
  await fetchOrders()
})
</script>

<template>
  <div class="orders-wrapper">
    <div class="orders-container">
      <div class="orders-header">
        <h1>Заказы</h1>
      </div>
      
      <div class="orders-stats" v-if="orders.length > 0">
        <div class="stats-row">
          <span class="stat-item">
            Всего заказов: <strong>{{ orders.length }}</strong>
          </span>
          <span class="stat-item">
            Новых: <strong>{{ newOrdersCount }}</strong>
          </span>
          <span class="stat-item">
            Завершенных: <strong>{{ completedOrdersCount }}</strong>
          </span>
        </div>
        
        <div class="filter-row">
          <label for="status-filter">Фильтр по статусу:</label>
          <select id="status-filter" v-model="selectedStatus" class="status-filter">
            <option value="all">Все заказы</option>
            <option value="Новый">Новые</option>
            <option value="Завершен">Завершенные</option>
          </select>
        </div>
      </div>
      
      <div v-if="isLoading" class="loading">
        <div class="spinner"></div>
        <p>Загрузка заказов...</p>
      </div>
      
      <div v-else-if="error" class="error-message">
        <p>{{ error }}</p>
        <button @click="fetchOrders" class="btn-retry">Повторить</button>
      </div>
      
      <div v-else-if="filteredOrders.length === 0" class="empty-orders">
        <div class="empty-orders-icon">📦</div>
        <h2>Заказов не найдено</h2>
        <p v-if="selectedStatus !== 'all'">Попробуйте выбрать другой статус</p>
        <p v-else-if="userRole === 'Пользователь'">
          Перейдите в каталог, чтобы сделать первый заказ
        </p>
        <button v-if="userRole === 'Пользователь'" @click="router.push('/home')" class="btn-shop">
          Перейти к покупкам
        </button>
      </div>
      
      <div v-else class="orders-list">
        <div v-for="order in filteredOrders" :key="order.orderId" class="order-card">
          <div class="order-header">
            <div class="order-info">
              <span class="order-number">Заказ #{{ order.orderId }}</span>
              <span class="order-date">{{ formatDate(order.orderDate) }}</span>
              <span v-if="order.orderCode" class="order-code">
                Код: {{ order.orderCode }}
              </span>
              <span v-if="canViewAllOrders && order.user" class="order-user">
                Клиент: {{ order.user.userSurname || '' }} {{ order.user.userName || '' }}
              </span>
              <span v-if="order.pickUpPoint" class="order-pickup">
                Пункт выдачи: {{ order.pickUpPoint.pickUpPointAdress }}
              </span>
            </div>
            
            <div class="order-status">
              <span 
                class="status-badge" 
                :class="{ 
                  'status-new': order.orderStatus === 'Новый', 
                  'status-completed': order.orderStatus === 'Завершен' 
                }"
              >
                {{ order.orderStatus }}
              </span>
              
              <button 
                v-if="canCompleteOrder && order.orderStatus === 'Новый'"
                @click="completeOrder(order.orderId)"
                class="complete-btn"
                title="Завершить заказ"
              >
                ✓ Завершить
              </button>
              
              <button 
                v-if="userRole === 'Пользователь' && order.orderStatus === 'Новый'"
                @click="cancelOrder(order.orderId)"
                class="cancel-btn"
                title="Отменить заказ"
              >
                ×
              </button>
            </div>
          </div>
          
          <div class="order-items">
            <table class="items-table">
              <thead>
                <tr>
                  <th>Товар</th>
                  <th>Артикул</th>
                  <th>Количество</th>
                  <th>Цена</th>
                  <th>Скидка</th>
                  <th>Сумма</th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="!order.structureOrders || order.structureOrders.length === 0">
                  <td colspan="6" class="no-items">Нет товаров в заказе</td>
                </tr>
                <tr v-for="item in order.structureOrders" :key="item.structureOrderId">
                  <td>
                    <div class="product-info">
                      <span class="product-name">{{ item.tovar?.tovarName || 'Товар удален' }}</span>
                      <span v-if="item.tovar?.categoryName" class="product-category">
                        {{ item.tovar.categoryName }}
                      </span>
                      <span v-if="item.tovar?.manufacturerName" class="product-manufacturer">
                        {{ item.tovar.manufacturerName }}
                      </span>
                    </div>
                  </td>
                  <td>{{ item.tovarArticle || '—' }}</td>
                  <td>{{ item.structureOrderTovarCount || 0 }} {{ item.tovar?.tovarUnit || 'шт.' }}</td>
                  <td>{{ (item.tovar?.tovarCost || 0).toFixed(2) }} ₽</td>
                  <td>
                    <span v-if="item.tovar?.tovarDiscount > 0" class="discount-badge">
                      -{{ item.tovar.tovarDiscount }}%
                    </span>
                    <span v-else>—</span>
                  </td>
                  <td class="total-cell">
                    {{ ((item.tovar?.tovarCost || 0) * (1 - (item.tovar?.tovarDiscount || 0) / 100) * (item.structureOrderTovarCount || 0)).toFixed(2) }} ₽
                  </td>
                </tr>
              </tbody>
              <tfoot>
                <tr>
                  <td colspan="5" class="total-label">Итого:</td>
                  <td class="total-value">{{ getOrderTotal(order).toFixed(2) }} ₽</td>
                </tr>
              </tfoot>
            </table>
          </div>
          
          <div v-if="order.orderDateDelivery" class="order-delivery">
            <span class="delivery-label">Дата доставки:</span>
            <span class="delivery-date">{{ formatDate(order.orderDateDelivery) }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.orders-wrapper {
  min-height: 100vh;
  background: white;
  padding: 80px 20px 40px;
}

.orders-container {
  max-width: 1200px;
  margin: 0 auto;
  background: white;
  border-radius: 16px;
  box-shadow: 0 8px 30px rgba(255, 126, 179, 0.1);
  overflow: hidden;
}

.orders-header {
  padding: 30px 40px;
  background: linear-gradient(135deg, #ff4081 0%, #ff7eb3 100%);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.orders-header h1 {
  font-size: 24px;
  font-weight: 600;
  margin: 0;
}

.header-info {
  padding: 8px 16px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 8px;
  font-weight: 500;
}

.orders-stats {
  padding: 20px 40px;
  background: #fff9fc;
  border-bottom: 1px solid #ffd6e7;
}

.stats-row {
  display: flex;
  gap: 30px;
  margin-bottom: 15px;
  flex-wrap: wrap;
}

.stat-item {
  font-size: 15px;
  color: #666;
}

.stat-item strong {
  color: #ff4081;
  font-size: 18px;
  margin-left: 5px;
}

.filter-row {
  display: flex;
  align-items: center;
  gap: 15px;
  flex-wrap: wrap;
}

.filter-row label {
  font-weight: 600;
  color: #ff4081;
}

.status-filter {
  padding: 8px 15px;
  border: 2px solid #ffd6e7;
  border-radius: 8px;
  font-size: 14px;
  background: white;
  cursor: pointer;
  transition: all 0.3s ease;
  min-width: 150px;
}

.status-filter:focus {
  outline: none;
  border-color: #ff4081;
}

.empty-orders {
  padding: 80px 40px;
  text-align: center;
}

.empty-orders-icon {
  font-size: 80px;
  margin-bottom: 20px;
  opacity: 0.5;
}

.empty-orders h2 {
  color: #333;
  margin-bottom: 10px;
}

.empty-orders p {
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

.error-message {
  padding: 60px 40px;
  text-align: center;
  color: #f44336;
}

.btn-retry {
  margin-top: 20px;
  padding: 10px 30px;
  background: #ff4081;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s ease;
}

.btn-retry:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 64, 129, 0.3);
}

.orders-list {
  padding: 40px;
  display: flex;
  flex-direction: column;
  gap: 30px;
}

.order-card {
  background: #f9f9f9;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid #e0e0e0;
  transition: all 0.3s ease;
}

.order-card:hover {
  box-shadow: 0 8px 25px rgba(255, 64, 129, 0.1);
  border-color: #ff7eb3;
}

.order-header {
  padding: 20px;
  background: white;
  border-bottom: 2px solid #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 15px;
}

.order-info {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
  align-items: center;
}

.order-number {
  font-weight: 600;
  color: #ff4081;
}

.order-date {
  color: #666;
  font-size: 14px;
}

.order-code {
  color: #4CAF50;
  font-size: 14px;
  font-weight: 600;
  background: #e8f5e9;
  padding: 4px 8px;
  border-radius: 4px;
}

.order-user {
  color: #333;
  font-size: 14px;
  background: #e0e0e0;
  padding: 4px 8px;
  border-radius: 4px;
}

.order-pickup {
  color: #ff7eb3;
  font-size: 14px;
  background: #fff5f7;
  padding: 4px 8px;
  border-radius: 4px;
  max-width: 300px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.order-status {
  display: flex;
  align-items: center;
  gap: 10px;
}

.status-badge {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 600;
}

.status-new {
  background: #ff4081;
  color: white;
}

.status-completed {
  background: #4CAF50;
  color: white;
}

.complete-btn {
  padding: 6px 12px;
  background: #4CAF50;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.complete-btn:hover {
  background: #45a049;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(76, 175, 80, 0.3);
}

.cancel-btn {
  width: 30px;
  height: 30px;
  background: #f44336;
  color: white;
  border: none;
  border-radius: 50%;
  font-size: 18px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.cancel-btn:hover {
  background: #d32f2f;
  transform: scale(1.1);
}

.order-items {
  padding: 20px;
  overflow-x: auto;
}

.items-table {
  width: 100%;
  border-collapse: collapse;
  min-width: 800px;
}

.items-table th {
  text-align: left;
  padding: 10px;
  background: white;
  color: #666;
  font-weight: 600;
  font-size: 14px;
  border-bottom: 2px solid #e0e0e0;
}

.items-table td {
  padding: 12px 10px;
  border-bottom: 1px solid #e0e0e0;
  color: #333;
  vertical-align: middle;
}

.items-table tfoot tr {
  background: white;
}

.items-table tfoot td {
  padding: 15px 10px;
  font-weight: 600;
}

.no-items {
  text-align: center;
  padding: 30px;
  color: #999;
  font-style: italic;
}

.product-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.product-name {
  font-weight: 600;
  color: #333;
}

.product-category {
  font-size: 12px;
  color: #ff7eb3;
}

.product-manufacturer {
  font-size: 12px;
  color: #666;
}

.discount-badge {
  display: inline-block;
  padding: 2px 6px;
  background: #ff4081;
  color: white;
  border-radius: 4px;
  font-size: 11px;
  font-weight: 600;
}

.total-cell {
  font-weight: 600;
  color: #ff4081;
}

.total-label {
  text-align: right;
  color: #ff4081;
  font-size: 16px;
}

.total-value {
  color: #ff4081;
  font-size: 18px;
  font-weight: 700;
}

.order-delivery {
  padding: 15px 20px;
  background: #fff3e0;
  border-top: 1px solid #ffe0b2;
  display: flex;
  gap: 10px;
  align-items: center;
}

.delivery-label {
  font-weight: 600;
  color: #ff9800;
}

.delivery-date {
  color: #333;
  font-weight: 500;
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

@media (max-width: 768px) {
  .orders-wrapper {
    padding: 70px 15px 30px;
  }
  
  .orders-header {
    padding: 20px 25px;
  }
  
  .orders-stats {
    padding: 15px 20px;
  }
  
  .stats-row {
    gap: 15px;
  }
  
  .orders-list {
    padding: 20px;
  }
  
  .order-header {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .order-info {
    width: 100%;
  }
  
  .order-pickup {
    max-width: 100%;
  }
  
  .order-status {
    width: 100%;
    justify-content: flex-start;
  }
  
  .items-table {
    font-size: 12px;
    min-width: 600px;
  }
  
  .items-table th,
  .items-table td {
    padding: 8px 5px;
  }
}
</style>