<template>
  <header class="header">
    <div class="top">
      <div class="column logo-column">
        <div class="logo">
          <img src="/resources/cosmetic_logo.png" alt="Логотип Косметика">
        </div>
      </div>
      
      <div class="column title-column">
        <h1 class="title_parfum">ООО Косметика</h1>
      </div>
      
      <div class="column user-column">
        <!-- Для авторизованного пользователя -->
        <div class="user-info" v-if="userData">
          <div class="action-buttons">
            <!-- Кнопка корзины только для обычных пользователей (не админ и не менеджер) -->
            <button 
              v-if="userData.userRole !== 'Администратор' && userData.userRole !== 'Менеджер'" 
              @click="goToCart" 
              class="cart-btn"
            >
              🛒 Корзина
              <span v-if="cartCount > 0" class="cart-badge">{{ cartCount }}</span>
            </button>
            
            <!-- Кнопка заказов для менеджера и админа -->
            <button v-if="userData.userRole === 'Администратор' || userData.userRole === 'Менеджер'" @click="goToOrders" class="orders-btn">
              📋 Заказы
            </button>
            
            <!-- Кнопка добавления товара для админа -->
            <button v-if="userData.userRole === 'Администратор'" @click="goToAddProduct" class="add-product-btn">
              ➕ Добавить товар
            </button>
            
            <!-- Кнопка выхода -->
            <button @click="logout" class="logout-btn">
              Выйти
            </button>
          </div>
          
          <div class="user-details">
            <span class="user-role">{{ userData.userRole }}: {{ userData.userSurname }} {{ userData.userName }}</span>
          </div>
        </div>
        
        <!-- Для гостя - только кнопка авторизации -->
        <div class="menu" v-else>
          <RouterLink to="/" class="auth-link">Авторизация</RouterLink>
        </div>
      </div>
    </div>
  </header>
  <RouterView class="content"/>
</template>

<script setup>
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { ref, onMounted, onUnmounted } from 'vue';
import { useCookies } from 'vue3-cookies';
import axios from 'axios';

const { cookies } = useCookies();
const userData = ref(null);
const router = useRouter();
const cartCount = ref(0);

// Функция для получения данных пользователя из cookies
const getUserFromCookies = () => {
  try {
    const user = cookies.get('user');
    return user || null;
  } catch (e) {
    console.error('Ошибка при получении user из cookies:', e);
    return null;
  }
};

// Обновление счетчика корзины из БД
const updateCartCount = async () => {
  if (userData.value) {
    try {
      const response = await axios.get(`http://localhost:5122/api/Cart/GetCart/${userData.value.userId}`);
      if (response.data && Array.isArray(response.data)) {
        cartCount.value = response.data.reduce((sum, item) => sum + (item.cartTovarCount || 0), 0);
      } else {
        cartCount.value = 0;
      }
    } catch (error) {
      console.error('Ошибка при загрузке корзины:', error);
      cartCount.value = 0;
    }
  } else {
    cartCount.value = 0;
  }
};

// Обновление данных пользователя
const updateUserData = () => {
  const newUserData = getUserFromCookies();
  
  // Сравниваем старые и новые данные
  if (JSON.stringify(userData.value) !== JSON.stringify(newUserData)) {
    console.log('Данные пользователя изменились:', newUserData);
    userData.value = newUserData;
    // Обновляем корзину при изменении пользователя
    updateCartCount();
  }
};

// Выход
const logout = () => {
  cookies.remove('user');
  cookies.remove('guest');
  userData.value = null;
  cartCount.value = 0;
  router.push('/');
};

// Навигация
const goToCart = () => {
  router.push('/cart');
};

const goToOrders = () => {
  router.push('/orders');
};

const goToAddProduct = () => {
  router.push('/product/add');
};

// Проверка изменений в cookies каждые 500ms
let interval;
onMounted(() => {
  updateUserData();
  
  // Устанавливаем интервал для проверки изменений в cookies
  interval = setInterval(() => {
    updateUserData();
  }, 500);
  
  // Слушаем изменения в localStorage (для совместимости)
  window.addEventListener('storage', (e) => {
    if (e.key && e.key.startsWith('cart_')) {
      updateCartCount();
    }
  });
});

onUnmounted(() => {
  if (interval) {
    clearInterval(interval);
  }
});
</script>

<style scoped>
.header {
  background: linear-gradient(135deg, #ffb8d9 0%, #ffd6e7 100%);
  padding: 10px 20px;
  box-shadow: 0 4px 12px rgba(255, 184, 217, 0.2);
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
  border-bottom: 1px solid rgba(255, 255, 255, 0.3);
}

.top {
  display: grid;
  grid-template-columns: 1fr 3fr 1.5fr;
  align-items: center;
  max-width: 1200px;
  margin: 0 auto;
  gap: 20px;
}

.column {
  display: flex;
  align-items: center;
  height: 100%;
}

.logo-column {
  justify-content: flex-start;
}

.logo {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  overflow: hidden;
  background-color: rgba(255, 255, 255, 0.2);
  border: 2px solid rgba(255, 255, 255, 0.4);
  box-shadow: 0 2px 8px rgba(255, 184, 217, 0.2);
}

.logo img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  display: block;
}

.title-column {
  justify-content: center;
  text-align: center;
}

.title_parfum {
  font-size: 28px;
  font-weight: 600;
  color: #d14a7c;
  margin: 0;
  text-shadow: 0 1px 2px rgba(255, 255, 255, 0.5);
}

.user-column {
  justify-content: flex-end;
}

.user-info {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 5px;
  max-width: 100%;
}

.action-buttons {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  justify-content: flex-end;
  margin-bottom: 5px;
}

.action-buttons button {
  margin-bottom: 0;
}

.user-details {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  color: #d14a7c;
  font-size: 14px;
  background-color: rgba(255, 255, 255, 0.2);
  padding: 8px 15px;
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.4);
  backdrop-filter: blur(5px);
  text-align: right;
  max-width: 100%;
  word-break: break-word;
}

.user-role {
  font-weight: 500;
  color: #a53a64;
}

/* Кнопки */
.cart-btn, .orders-btn, .add-product-btn, .logout-btn {
  padding: 6px 12px;
  border: none;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  white-space: nowrap;
  position: relative;
}

.cart-btn {
  background: linear-gradient(135deg, #ff4081 0%, #ff7eb3 100%);
  color: white;
  padding-right: 25px;
}

.cart-badge {
  position: absolute;
  top: -5px;
  right: -5px;
  background: #f44336;
  color: white;
  font-size: 10px;
  font-weight: bold;
  min-width: 16px;
  height: 16px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 4px;
}

.orders-btn {
  background: #4CAF50;
  color: white;
}

.add-product-btn {
  background: #2196F3;
  color: white;
}

.logout-btn {
  background-color: rgba(255, 255, 255, 0.25);
  color: #d14a7c;
  border: 1px solid rgba(255, 255, 255, 0.4);
}

.cart-btn:hover, .orders-btn:hover, .add-product-btn:hover, .logout-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(255, 184, 217, 0.3);
}

.menu {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  width: 100%;
}

.menu a {
  color: #d14a7c;
  text-decoration: none;
  font-size: 16px;
  font-weight: 600;
  padding: 10px 20px;
  background-color: rgba(255, 255, 255, 0.25);
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.4);
  transition: all 0.3s ease;
  text-align: center;
}

.menu a:hover {
  background-color: rgba(255, 255, 255, 0.4);
  color: #c03d70;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(255, 184, 217, 0.3);
  cursor: pointer;
}

.auth-link {
  background-color: rgba(255, 255, 255, 0.3) !important;
}

.content {
  margin-top: 100px;
}

@media (max-width: 768px) {
  .top {
    grid-template-columns: 1fr 2fr 1fr;
    gap: 10px;
  }
  
  .logo {
    width: 50px;
    height: 50px;
  }
  
  .title_parfum {
    font-size: 22px;
  }
  
  .user-details {
    font-size: 12px;
    padding: 6px 10px;
  }
  
  .action-buttons {
    gap: 5px;
  }
  
  .cart-btn, .orders-btn, .add-product-btn, .logout-btn {
    padding: 4px 8px;
    font-size: 11px;
  }
  
  .menu a {
    padding: 8px 15px;
    font-size: 14px;
  }
}
</style>