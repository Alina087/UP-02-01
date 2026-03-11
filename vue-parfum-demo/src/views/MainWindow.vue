<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import axios from 'axios'
import cardItem from '@/components/goodCard.vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const products = ref([])
const suppliers = ref([])
const allProducts = ref([])
const userRole = ref('') 

const searchText = ref('')
const selectedSupplier = ref('all')
const sortField = ref('name')
const sortDirection = ref('asc')
const c = ref('')


// Загрузка товаров
const fetchProducts = async () => {
  try {
    const response = await axios.get('http://localhost:5122/api/Tovar/GetTovar') 
    allProducts.value = response.data
    
    applyFilters()
  } catch (error) {
    console.error('Ошибка при загрузке товаров:', error)
  }
}


// Загрузка поставщиков
const fetchSuppliers = async () => {
  try {
    const response = await axios.get('http://localhost:5122/api/Tovar/GetSuppliers')
    suppliers.value = response.data
  } catch (error) {
    console.error('Ошибка при загрузке поставщиков:', error)
  }
}

// Получение роли пользователя из cookies
const getUserRole = () => {
  const userCookie = document.cookie
    .split('; ')
    .find(row => row.startsWith('user='))
    
  if (!userCookie) return 'Гость'
  
  try {
    const userData = JSON.parse(decodeURIComponent(userCookie.split('=')[1]))
    return userData.userRole || 'Гость'
  } catch (e) {
    console.error('Ошибка при парсинге user cookie:', e)
    return 'Гость'
  }
}

// Проверка прав на фильтрацию и сортировку
const canFilterAndSort = computed(() => {
  const role = userRole.value
  return role === 'Администратор' || role === 'Менеджер'
})

// Проверка прав на управление товарами
const canManageProducts = computed(() => {
  return userRole.value === 'Администратор'
})

// Проверка является ли гостем
const isGuest = computed(() => {
  return userRole.value === 'Гость' || !userRole.value
})

// Функция фильтрации и сортировки
const applyFilters = () => {
  let filtered = [...allProducts.value]
  
  // Если пользователь не имеет прав на фильтрацию, показываем все товары без фильтров
  if (!canFilterAndSort.value) {
    products.value = filtered
    return
  }
  
  // Поиск по всем текстовым полям
  if (searchText.value.trim() !== '') {
    const searchTerm = searchText.value.toLowerCase().trim()
    filtered = filtered.filter(product => {
      return (
        (product.tovarName?.toLowerCase() || '').includes(searchTerm) ||
        (product.tovarDescription?.toLowerCase() || '').includes(searchTerm) ||
        (product.supplierName?.toLowerCase() || '').includes(searchTerm) ||
        (product.manufacturerName?.toLowerCase() || '').includes(searchTerm) ||
        (product.tovarCategoryName?.toLowerCase() || '').includes(searchTerm)
      )
    })
  }
  
  // Фильтрация по поставщику
  if (selectedSupplier.value !== 'all') {
    const supplierId = parseInt(selectedSupplier.value)
    filtered = filtered.filter(product => product.supplierId === supplierId)
  }
  
  // Сортировка
  filtered.sort((a, b) => {
    let aValue, bValue
    
    if (sortField.value === 'name') {
      aValue = a.tovarName || ''
      bValue = b.tovarName || ''
    } else if (sortField.value === 'count') {
      aValue = a.tovarCount || 0
      bValue = b.tovarCount || 0
    } else if (sortField.value === 'cost') {
      aValue = a.tovarCost || 0
      bValue = b.tovarCost || 0
    }
    
    if (typeof aValue === 'string' && typeof bValue === 'string') {
      return sortDirection.value === 'asc' 
        ? aValue.localeCompare(bValue)
        : bValue.localeCompare(aValue)
    } else {
      return sortDirection.value === 'asc'
        ? aValue - bValue
        : bValue - aValue
    }
  })
  
  products.value = filtered
  c.value = products.value.length
}

// Переключение сортировки
const toggleSort = (field) => {
  if (!canFilterAndSort.value) return
  
  if (sortField.value === field) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortField.value = field
    sortDirection.value = 'asc'
  }
  applyFilters()
}

// Удаление товара
const deleteProduct = async (article) => {
  if (!canManageProducts.value) return
  
  if (!confirm('Вы уверены, что хотите удалить этот товар?')) {
    return
  }
  
  try {
    await axios.delete(`http://localhost:5122/api/Tovar/DeleteTovar?article=${article}`)
    // Обновляем список товаров
    fetchProducts()
  } catch (error) {
    console.error('Ошибка при удалении товара:', error)
    if (error.response?.status === 400 && error.response?.data.includes('заказе')) {
      alert('Товар нельзя удалить, так как он присутствует в заказе')
    } else {
      alert('Ошибка при удалении товара')
    }
  }
}

// Редактирование товара
const editProduct = (product) => {
  if (!canManageProducts.value) return
  router.push(`/product/edit/${product.tovarArticle}`)
}

// Добавление товара
const addProduct = () => {
  if (!canManageProducts.value) return
  router.push('/product/add')
}

// Клик по карточке товара
const handleProductClick = (product) => {
  // Только для администратора - редактирование
  if (canManageProducts.value) {
    router.push(`/product/edit/${product.tovarArticle}`)
  }
  // Для остальных - ничего не делаем
}

// Наблюдаем за изменениями фильтров
watch([searchText, selectedSupplier], applyFilters)

onMounted(() => {
  fetchProducts()
  fetchSuppliers()
  userRole.value = getUserRole()
  console.log('Текущая роль пользователя:', userRole.value)
})
</script>

<template>
  <div class="catalog-wrapper">
    <div class="catalog">
      <div class="catalog-header">
        <h1>Каталог товаров</h1>
        
        <!-- Панель фильтров и сортировки (только для менеджера и админа) -->
        <div v-if="canFilterAndSort" class="controls-panel">
          <!-- Левая часть: поиск и фильтры -->
          <div class="controls-left">
            <!-- Поиск -->
            <div class="search-container">
              <input
                v-model="searchText"
                type="text"
                placeholder="Поиск по товарам..."
                class="search-input"
              />
              <span class="search-icon">🔍</span>
            </div>
            
            <!-- Фильтр по поставщику -->
            <div class="filter-container">
              <select v-model="selectedSupplier" class="filter-select">
                <option value="all">Все поставщики</option>
                <option 
                  v-for="supplier in suppliers" 
                  :key="supplier.supplierId"
                  :value="supplier.supplierId"
                >
                  {{ supplier.supplierName }}
                </option>
              </select>
            </div>

           
            
            <!-- Сортировка -->
            <div class="sort-container">              
              <button 
                @click="toggleSort('count')"
                class="sort-btn"
                :class="{ active: sortField === 'count' }"
              >
                По количеству
                <span v-if="sortField === 'count'" class="sort-arrow">
                  {{ sortDirection === 'asc' ? '↑' : '↓' }}
                </span>
              </button>
            </div>
          </div>

           <div class="countt">
              Количество товаров: {{ products.length }}
            </div>
          
          <div v-if="canManageProducts" class="controls-right">
            <button @click="addProduct" class="add-btn">
              <span class="add-icon">+</span>
              Добавить товар
            </button>
          </div>
        </div>
      </div>
      
      <div class="products-grid">
        <div 
          v-for="product in products" 
          :key="product.tovarArticle" 
          class="product-card-wrapper"
          :class="{ 'clickable': canManageProducts }"
        >
          <cardItem
            :product="product"
            @click="handleProductClick(product)"
            :class="{ 'cursor-default': !canManageProducts }"
          />
          
          <!-- Кнопки действий (только для администратора) -->
          <div v-if="canManageProducts" class="card-actions">
            <button @click.stop="editProduct(product)" class="action-btn edit-btn">
              Редактировать
            </button>
            <button @click.stop="deleteProduct(product.tovarArticle)" class="action-btn delete-btn">
              Удалить
            </button>
          </div>
        </div>
      </div>
      
      <div v-if="products.length === 0" class="no-results">
        <p v-if="canFilterAndSort">Товары не найдены. Попробуйте изменить параметры поиска.</p>
        <p v-else>Товары не найдены.</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.catalog-wrapper {
  min-height: 100vh;
  width: 100%;
  background: white;
  margin: 0;
  padding: 0;
}

.catalog {
  width: 100%;
  min-height: 100vh;
  padding: 120px 0 40px;
  margin: 0;
  box-sizing: border-box;
}

.catalog-header {
  margin-bottom: 40px;
  max-width: 1200px;
  margin-left: auto;
  margin-right: auto;
  padding: 0 20px;
}

.catalog-header h1 {
  font-size: 36px;
  font-weight: 700;
  color: #ff7eb3;
  text-align: center;
  margin-bottom: 30px;
  position: relative;
  display: inline-block;
  width: 100%;
}

.catalog-header h1:after {
  content: '';
  position: absolute;
  bottom: -10px;
  left: 50%;
  transform: translateX(-50%);
  width: 100px;
  height: 3px;
  background: linear-gradient(90deg, #ff7eb3, #ffb8d9);
  border-radius: 2px;
}

/* Панель управления */
.controls-panel {
  background: #fff9fc;
  border: 1px solid #ffd6e7;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
  box-shadow: 0 4px 15px rgba(255, 184, 217, 0.1);
}

.controls-left {
  display: flex;
  flex: 1;
  gap: 15px;
  align-items: center;
  flex-wrap: wrap;
}

.controls-right {
  display: flex;
  align-items: center;
}

.search-container {
  position: relative;
  min-width: 200px;
}

.search-input {
  width: 100%;
  padding: 12px 40px 12px 15px;
  border: 2px solid #ffd6e7;
  border-radius: 8px;
  font-size: 14px;
  transition: all 0.3s ease;
  background: white;
}

.search-input:focus {
  outline: none;
  border-color: #ff7eb3;
  box-shadow: 0 0 0 3px rgba(255, 126, 179, 0.1);
}

.search-icon {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  color: #ff7eb3;
}

.filter-select {
  padding: 12px 15px;
  border: 2px solid #ffd6e7;
  border-radius: 8px;
  font-size: 14px;
  background: white;
  cursor: pointer;
  transition: all 0.3s ease;
  min-width: 150px;
}

.filter-select:focus {
  outline: none;
  border-color: #ff7eb3;
}

.sort-container {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.sort-btn {
  padding: 8px 12px;
  border: 2px solid #ffd6e7;
  background: white;
  border-radius: 8px;
  cursor: pointer;
  font-size: 12px;
  font-weight: 500;
  color: #666;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 5px;
  white-space: nowrap;
}

.sort-btn:hover {
  border-color: #ff7eb3;
  color: #ff7eb3;
}

.sort-btn.active {
  background: linear-gradient(135deg, #ff4081 0%, #ff7eb3 100%);
  color: white;
  border-color: #ff4081;
}

.sort-arrow {
  font-weight: bold;
}

.add-btn {
  padding: 10px 20px;
  background: linear-gradient(135deg, #ff4081 0%, #ff7eb3 100%);
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  transition: all 0.3s ease;
  white-space: nowrap;
  display: flex;
  align-items: center;
  gap: 8px;
  height: 42px;
}

.add-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 64, 129, 0.3);
}

.add-icon {
  font-size: 18px;
  font-weight: bold;
}

/* Информация для гостей */
.guest-info {
  background: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  padding: 15px 20px;
  margin-bottom: 30px;
  text-align: center;
}

.info-text {
  color: #6c757d;
  font-size: 14px;
  margin: 0;
}

/* Обертка для карточки товара */
.product-card-wrapper {
  position: relative;
  transition: all 0.3s ease;
}

.product-card-wrapper.clickable:hover {
  transform: translateY(-5px);
  cursor: pointer;
}

/* Отключаем курсор для не-админов */
.cursor-default {
  cursor: default !important;
}

.card-actions {
  position: absolute;
  bottom: 20px;
  right: 20px;
  display: flex;
  gap: 10px;
  z-index: 10;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.product-card-wrapper:hover .card-actions {
  opacity: 1;
}

.action-btn {
  padding: 6px 12px;
  border: none;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  white-space: nowrap;
}

.edit-btn {
  background: #4CAF50;
  color: white;
}

.edit-btn:hover {
  background: #45a049;
  transform: translateY(-2px);
}

.delete-btn {
  background: #f44336;
  color: white;
}

.delete-btn:hover {
  background: #d32f2f;
  transform: translateY(-2px);
}

.products-grid {
  display: grid;
  gap: 25px;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

.no-results {
  text-align: center;
  padding: 40px;
  color: #666;
  font-size: 16px;
}

/* Адаптивность */
@media (max-width: 1024px) {
  .controls-panel {
    flex-direction: column;
    align-items: stretch;
  }
  
  .controls-left {
    flex-direction: column;
    align-items: stretch;
  }
  
  .search-container,
  .filter-container,
  .sort-container {
    width: 100%;
  }
  
  .search-input,
  .filter-select {
    width: 100%;
  }
  
  .sort-container {
    justify-content: center;
  }
  
  .controls-right {
    width: 100%;
    justify-content: center;
  }
  
  .add-btn {
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 768px) {
  .sort-container {
    flex-direction: column;
  }
  
  .sort-btn {
    width: 100%;
    justify-content: center;
  }
  
  .catalog-header h1 {
    font-size: 28px;
  }
  
  .products-grid {
    gap: 15px;
  }
  
  .card-actions {
    position: static;
    opacity: 1;
    margin-top: 10px;
    display: flex;
    justify-content: center;
  }
}
</style>