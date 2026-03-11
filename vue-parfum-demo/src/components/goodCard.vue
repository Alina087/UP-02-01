<!-- goodCard.vue -->
<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useCookies } from 'vue3-cookies'
import axios from 'axios'

const props = defineProps({
  product: {
    type: Object,
    required: true,
    default: () => ({})
  }
})

const router = useRouter()
const { cookies } = useCookies()

const imagePath = computed(() => {
  const image = props.product.tovarImage || 'picture.png'
  return `/resources/${image}`
})

// Вычисление цены со скидкой
const discountedPrice = computed(() => {
  const cost = props.product.tovarCost
  const discount = props.product.tovarDiscount
  
  if (discount > 0) {
    return Math.round(cost * (1 - discount / 100))
  }
  return cost
})

const hasDiscount = computed(() => {
  return (props.product.tovarDiscount || 0) > 0
})

const isBigDiscount = computed(() => {
  return (props.product.tovarDiscount || 0) > 15
})

const isOutOfStock = computed(() => {
  return (props.product.tovarCount || 0) <= 0
})

const cardClass = computed(() => {
  if (isOutOfStock.value) {
    return 'out-of-stock'
  } else if (isBigDiscount.value) {
    return 'big-discount'
  }
  return ''
})

const backgroundColor = computed(() => {
  if (isOutOfStock.value) {
    return '#E6F3FF'
  } else if (isBigDiscount.value) {
    return '#2E8B57'
  }
  return '' 
})

// Проверка авторизации пользователя
const isAuthenticated = computed(() => {
  const user = cookies.get('user')
  return !!user
})

// Получение данных пользователя
const userData = computed(() => {
  return cookies.get('user')
})

// Добавление в корзину через API
const addToCart = async (event) => {
  event.stopPropagation()
  
  if (!isAuthenticated.value) {
    alert('Необходимо авторизоваться для добавления товаров в корзину')
    router.push('/login')
    return
  }

  if (isOutOfStock.value) {
    alert('Товара нет в наличии')
    return
  }

  try {
    const cartItem = {
      userId: userData.value.userId,
      tovarArticle: props.product.tovarArticle,
      cartTovarCount: 1
    }

    console.log('Отправляем в корзину:', JSON.stringify(cartItem))
    
    const response = await axios.post('http://localhost:5122/api/Cart/AddToCart', cartItem, {
      headers: {
        'Content-Type': 'application/json'
      }
    })
    
    console.log('Ответ от сервера:', response.data)
    
    if (typeof response.data === 'string') {
      alert(response.data)
    } else if (response.data && response.data.message) {
      alert(response.data.message)
    } else {
      alert('Товар добавлен в корзину')
    }
    
    // Триггерим событие для обновления счетчика в хедере
    window.dispatchEvent(new Event('storage'))
    
  } catch (error) {
    console.error('Ошибка при добавлении в корзину:', error)
    
    if (error.response) {
      console.error('Статус ошибки:', error.response.status)
      console.error('Детали ошибки:', error.response.data)
      
      let errorMessage = 'Ошибка при добавлении в корзину'
      
      if (typeof error.response.data === 'string') {
        errorMessage = error.response.data
      } else if (error.response.data && typeof error.response.data === 'object') {
        errorMessage = error.response.data.message || 
                       error.response.data.error || 
                       error.response.data.title ||
                       JSON.stringify(error.response.data)
      }
      
      alert(errorMessage)
    } else if (error.request) {
      alert('Сервер не отвечает. Проверьте подключение.')
    } else {
      alert('Ошибка при добавлении в корзину')
    }
  }
}

const isRegularUser = computed(() => {
  if (!userData.value) return false
  return userData.value.userRole !== 'Администратор' && userData.value.userRole !== 'Менеджер'
})
</script>

<template>
  <div 
    class="product-card" 
    v-if="product"
    :class="cardClass"
    :style="{ backgroundColor: backgroundColor }"
  >
    <div class="column image-column">
      <div class="product-image">
        <img 
          :src="imagePath" 
          :alt="product.tovarName"
          class="perfume-image"
          @error="(e) => { e.target.src = '/resources/picture.png' }"
        />
      </div>
    </div>
    
    <div class="column info-column">
      <div class="category-row">
        <span class="product-category">{{ product.tovarCategoryName }}</span>
        <span class="separator">|</span>
        <span class="product-name">{{ product.tovarName }}</span>
      </div>
      
      <div class="description-row">
        <span class="label">Описание товара:</span>
        <span class="value">{{ product.tovarDescription }}</span>
      </div>
      
      <div class="manufacturer-row">
        <span class="label">Производитель:</span>
        <span class="value">{{ product.manufacturerName}}</span>
      </div>
      
      <div class="supplier-row">
        <span class="label">Поставщик:</span>
        <span class="value">{{ product.supplierName }}</span>
      </div>
      
      <div class="price-row">
        <span class="label">Цена:</span>
        <div class="price-value">
          <template v-if="hasDiscount">
            <span class="original-price">{{ (product.tovarCost).toLocaleString() }}₽</span>
            <span class="arrow">→</span>
            <span class="discounted-price">{{ discountedPrice.toLocaleString() }}₽</span>
          </template>
          <template v-else>
            <span class="current-price">{{ (product.tovarCost).toLocaleString() }}₽</span>
          </template>
        </div>
      </div>
      
      <div class="unit-info">
        <span class="label">Единица измерения:</span>
        <span class="value">{{ product.tovarUnit || 'шт.' }}</span>
      </div>
        
      <div class="stock-info">
        <span class="label">Количество на складе:</span>
        <span class="value">{{ product.tovarCount || 0 }}</span>
      </div>
      
      <!-- Кнопка "В корзину" только для авторизованных пользователей -->
      <div v-if="isAuthenticated && isRegularUser" class="cart-button-wrapper">
        <button 
          @click="addToCart"
          class="cart-btn"
          :disabled="isOutOfStock"
          :class="{ 'out-of-stock-btn': isOutOfStock }"
        >
          <span class="cart-icon">🛒</span>
          {{ isOutOfStock ? 'Нет в наличии' : 'В корзину' }}
        </button>
      </div>
    </div>
    
    <div class="column discount-column" v-if="hasDiscount">
      <div class="discount-circle">
        <div class="discount-percent">-{{ product.tovarDiscount }}%</div>
        <div class="discount-label">СКИДКА</div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Все стили остаются без изменений */
.product-card {
  display: grid;
  grid-template-columns: 150px 1fr 120px;
  gap: 20px;
  border: 1px solid #ffd6e7;
  border-radius: 16px;
  padding: 20px;
  background: white;
  transition: all 0.3s ease;
  min-height: 220px;
  align-self: center;
  height: 100%;
  margin: 0;
  box-shadow: 0 4px 15px rgba(255, 184, 217, 0.1);
  position: relative;
  overflow: hidden;
}

.product-card:hover {
  box-shadow: 0 8px 25px rgba(255, 184, 217, 0.2);
  transform: translateY(-5px);
  border-color: #ff7eb3;
}

.product-card.big-discount {
  border-color: #2E8B57;
  box-shadow: 0 4px 15px rgba(46, 139, 87, 0.2);
}

.product-card.big-discount:hover {
  box-shadow: 0 8px 25px rgba(46, 139, 87, 0.3);
  border-color: #2E8B57;
}

.product-card.out-of-stock {
  border-color: #87CEEB;
  box-shadow: 0 4px 15px rgba(135, 206, 235, 0.2);
}

.product-card.out-of-stock:hover {
  box-shadow: 0 8px 25px rgba(135, 206, 235, 0.3);
  border-color: #87CEEB;
}

.product-image {
  width: 100%;
  height: 160px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.perfume-image {
  width: 100%;
  height: 100%;
  object-fit: contain;
  border-radius: 12px;
  background: linear-gradient(135deg, #fff5f9 0%, #ffe6f0 100%);
  padding: 10px;
  border: 1px solid #ffe6f0;
}

.product-category {
  font-size: 14px;
  font-weight: 600;
  color: #ff7eb3;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.product-name {
  font-size: 16px;
  color: #d14a7c;
  font-weight: 600;
}

.description-row,
.manufacturer-row,
.supplier-row,
.price-row,
.unit-info,
.stock-info {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  font-size: 14px;
  line-height: 1.4;
}

.label {
  color: #ff7eb3;
  font-weight: 500;
  white-space: nowrap;
  min-width: 130px;
  flex-shrink: 0;
}

.value {
  color: #333;
  font-weight: 500;
  flex: 1;
  word-break: break-word;
}

.price-row {
  margin-top: 5px;
}

.original-price {
  font-size: 15px;
  color: #ffb8d9;
  text-decoration: line-through;
  font-weight: 500;
}

.arrow {
  color: #ff7eb3;
  font-size: 14px;
  font-weight: bold;
}

.discounted-price,
.current-price {
  font-size: 20px;
  font-weight: 700;
  color: #ff4081;
}

.unit-info,
.stock-info {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 13px;
  margin-top: 3px;
}

.unit-info .label,
.stock-info .label {
  min-width: auto;
  color: #ff7eb3;
}

.unit-info .value,
.stock-info .value {
  color: #e6729b;
  font-weight: 600;
}

.cart-button-wrapper {
  margin-top: 15px;
  display: flex;
  justify-content: flex-start;
}

.cart-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 20px;
  background: linear-gradient(135deg, #ff4081 0%, #ff7eb3 100%);
  color: white;
  border: none;
  border-radius: 25px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(255, 64, 129, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.cart-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 64, 129, 0.3);
}

.cart-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  background: #999;
}

.cart-btn.out-of-stock-btn {
  background: #999;
}

.cart-btn.out-of-stock-btn:hover {
  transform: none;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.cart-icon {
  font-size: 16px;
}

.column.discount-column {
  display: flex;
  align-items: center;
  justify-content: center;
  padding-left: 15px;
  border-left: 1px solid #ffe6f0;
}

.discount-circle {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 70px;
  height: 70px;
  border-radius: 50%;
  background: linear-gradient(135deg, #ff4081 0%, #ff7eb3 100%);
  color: white;
  box-shadow: 0 6px 20px rgba(255, 64, 129, 0.3);
}

.product-card.big-discount .discount-circle {
  background: linear-gradient(135deg, #2E8B57 0%, #3CB371 100%);
}

.product-card.out-of-stock .discount-circle {
  background: linear-gradient(135deg, #4A90E2 0%, #87CEEB 100%);
}

.discount-percent {
  font-size: 20px;
  font-weight: 700;
  line-height: 1;
}

.discount-label {
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.5px;
  margin-top: 2px;
  text-transform: uppercase;
}

@media (max-width: 768px) {
  .product-card {
    grid-template-columns: 1fr;
    gap: 15px;
  }
  
  .column.discount-column {
    border-left: none;
    border-top: 1px solid #ffe6f0;
    padding-left: 0;
    padding-top: 15px;
    justify-content: flex-start;
  }
  
  .discount-circle {
    width: 60px;
    height: 60px;
  }
  
  .cart-button-wrapper {
    justify-content: center;
  }
  
  .cart-btn {
    width: 100%;
    justify-content: center;
  }
}
</style>