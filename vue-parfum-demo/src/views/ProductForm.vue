<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import axios from 'axios'
import { useCookies } from 'vue3-cookies'

const route = useRoute()
const router = useRouter()
const { cookies } = useCookies()

const isEditMode = ref(false)
const productId = ref('')
const manufacturers = ref([])
const suppliers = ref([])
const categories = ref([])
const isLoading = ref(false)
const selectedFile = ref(null)
const imagePreview = ref(null)
const imageUrl = ref('')
const originalImageName = ref('')
const isSavingImage = ref(false)
const imageValidationError = ref('')

// Данные товара
const product = ref({
  tovarArticle: '',
  tovarName: '',
  tovarUnit: '',
  tovarCost: 0,
  supplierId: null,
  manufacturerId: null,
  tovarCategoryId: null,
  tovarDiscount: 0,
  tovarCount: 0,
  tovarDescription: '',
  tovarImage: ''
})

// Валидация полей
const validationErrors = ref({})

// Проверка, является ли пользователем администратором
const isAdmin = ref(false)

// Вычисляемое свойство для полного пути к изображению
const fullImagePath = computed(() => {
  if (product.value.tovarImage) {
    return `/resources/${product.value.tovarImage}`;
  }
  return '';
});

// ============================================
// ФУНКЦИИ ДЛЯ РАБОТЫ С ИЗОБРАЖЕНИЯМИ
// ============================================

/**
 * Проверяет размеры изображения
 */
const validateImageDimensions = (file) => {
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.onload = () => {
      if (img.width === 300 && img.height === 200) {
        resolve(true);
      } else {
        reject(`Изображение должно быть размером 300x200 пикселей (текущий: ${img.width}x${img.height})`);
      }
    };
    img.onerror = () => {
      reject('Не удалось загрузить изображение для проверки размеров');
    };
    img.src = URL.createObjectURL(file);
  });
};

/**
 * Генерирует уникальное имя файла на основе артикула
 */
const generateFileName = (article, file) => {
  if (!file) return '';
  
  const extension = file.name.split('.').pop().toLowerCase();
  const timestamp = Date.now();
  const random = Math.random().toString(36).substring(2, 8);
  
  if (article && article.trim()) {
    const cleanArticle = article.replace(/[^a-zA-Z0-9]/g, '_');
    return `${cleanArticle}_${timestamp}_${random}.${extension}`;
  }
  
  return `product_${timestamp}_${random}.${extension}`;
};

/**
 * Сохраняет файл в виртуальную папку resources (имитация)
 */
const saveFileToResourcesFolder = async (fileName, file) => {
  return new Promise((resolve, reject) => {
    try {
      const blob = new Blob([file], { type: file.type });
      const url = URL.createObjectURL(blob);
      
      const a = document.createElement('a');
      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      
      URL.revokeObjectURL(url);
      
      console.log(`Файл ${fileName} подготовлен для сохранения в папку resources`);
      
      const reader = new FileReader();
      reader.onload = () => {
        const fileData = {
          name: fileName,
          type: file.type,
          size: file.size,
          data: reader.result,
          date: new Date().toISOString()
        };
        
        const storedFiles = JSON.parse(localStorage.getItem('productImages') || '{}');
        storedFiles[fileName] = fileData;
        localStorage.setItem('productImages', JSON.stringify(storedFiles));
        
        resolve(fileName);
      };
      reader.onerror = reject;
      reader.readAsDataURL(file);
      
    } catch (error) {
      console.error('Ошибка при сохранении файла:', error);
      reject(error);
    }
  });
};

/**
 * Проверяет, существует ли файл в resources (имитация)
 */
const checkFileInResources = async (fileName) => {
  return new Promise((resolve) => {
    const img = new Image();
    img.onload = () => resolve(true);
    img.onerror = () => resolve(false);
    img.src = `/resources/${fileName}`;
  });
};

/**
 * Получает изображение из виртуальной папки или localStorage
 */
const getImageFromStorage = (fileName) => {
  try {
    const storedFiles = JSON.parse(localStorage.getItem('productImages') || '{}');
    if (storedFiles[fileName]) {
      return storedFiles[fileName];
    }
    
    return {
      name: fileName,
      data: `/resources/${fileName}`
    };
    
  } catch (error) {
    console.error('Ошибка при получении файла:', error);
    return null;
  }
};

/**
 * Удаляет файл из виртуальной папки (имитация)
 */
const deleteFileFromResources = async (fileName) => {
  if (!fileName) return;
  
  try {
    const storedFiles = JSON.parse(localStorage.getItem('productImages') || '{}');
    if (storedFiles[fileName]) {
      delete storedFiles[fileName];
      localStorage.setItem('productImages', JSON.stringify(storedFiles));
      console.log(`Файл ${fileName} удален из хранилища`);
    }
    
    console.log(`Файл ${fileName} помечен для удаления из папки resources`);
    
  } catch (error) {
    console.error('Ошибка при удалении файла:', error);
  }
};

/**
 * Обработчик выбора файла
 */
const handleFileSelect = async (event) => {
  const file = event.target.files[0];
  if (!file) return;
  
  try {
    isSavingImage.value = true;
    imageValidationError.value = '';
    
    // Проверяем тип файла
    const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    if (!validTypes.includes(file.type)) {
      alert('Пожалуйста, выберите файл изображения (JPEG, PNG, GIF, WebP)');
      return;
    }
    
    // Проверяем размер файла (макс 5MB)
    if (file.size > 5 * 1024 * 1024) {
      alert('Размер файла не должен превышать 5MB');
      return;
    }
    
    // Проверяем размеры изображения
    try {
      await validateImageDimensions(file);
    } catch (dimensionError) {
      imageValidationError.value = dimensionError;
      alert(dimensionError);
      return;
    }
    
    selectedFile.value = file;
    
    // Генерируем уникальное имя файла
    const fileName = generateFileName(product.value.tovarArticle, file);
    
    // Создаем превью
    const reader = new FileReader();
    reader.onload = (e) => {
      imagePreview.value = e.target.result;
    };
    reader.readAsDataURL(file);
    
    // Сохраняем файл в виртуальную папку resources
    await saveFileToResourcesFolder(fileName, file);
    
    // Запоминаем старое имя файла для удаления (если редактирование)
    const oldFileName = product.value.tovarImage;
    
    // Обновляем данные товара
    product.value.tovarImage = fileName;
    
    // Удаляем старое изображение, если оно было изменено
    if (isEditMode.value && oldFileName && oldFileName !== fileName) {
      await deleteFileFromResources(oldFileName);
    }
    
    console.log(`Изображение сохранено как: ${fileName}`);
    
  } catch (error) {
    console.error('Ошибка при обработке файла:', error);
    alert('Ошибка при обработке изображения');
  } finally {
    isSavingImage.value = false;
    event.target.value = '';
  }
};

/**
 * Очищает выбранное изображение
 */
const clearImage = async () => {
  const confirmed = confirm('Удалить изображение?')
  if (!confirmed) return
  
  try {
    if (product.value.tovarImage) {
      await deleteFileFromResources(product.value.tovarImage);
    }
    
    selectedFile.value = null;
    product.value.tovarImage = '';
    imagePreview.value = null;
    imageUrl.value = '';
    imageValidationError.value = '';
    
  } catch (error) {
    console.error('Ошибка при удалении изображения:', error);
    alert('Ошибка при удалении изображения');
  }
};

const validateProduct = () => {
  const errors = {};
  
  // Артикул
  if (!product.value.tovarArticle.trim()) {
    errors.tovarArticle = 'Артикул обязателен для заполнения';
  }

  if (product.value.tovarArticle.length < 6 || product.value.tovarArticle.length > 6){
    errors.tovarArticle = 'Длина артикула 6';
  }
  
  // Название
  if (!product.value.tovarName.trim()) {
    errors.tovarName = 'Название обязательно для заполнения';
  }
  
  // Стоимость
  if (product.value.tovarCost === null || product.value.tovarCost === '') {
    errors.tovarCost = 'Стоимость обязательна для заполнения';
  } else {
    const cost = Number(product.value.tovarCost);
    if (isNaN(cost)) {
      errors.tovarCost = 'Стоимость должна быть числом';
    } else if (cost < 1) {
      errors.tovarCost = 'Стоимость должна быть не менее 1';
    }
  }
  
  // Количество
  if (product.value.tovarCount === null || product.value.tovarCount === '') {
    errors.tovarCount = 'Количество обязательно для заполнения';
  } else {
    const count = Number(product.value.tovarCount);
    if (isNaN(count)) {
      errors.tovarCount = 'Количество должно быть числом';
    } else if (count < 1) {
      errors.tovarCount = 'Количество должно быть не менее 1';
    }
  }
  
  // Скидка
  if (product.value.tovarDiscount !== null && product.value.tovarDiscount !== '') {
    const discount = Number(product.value.tovarDiscount);
    if (isNaN(discount)) {
      errors.tovarDiscount = 'Скидка должна быть числом';
    } else if (discount < 0 || discount > 99) {
      errors.tovarDiscount = 'Скидка должна быть от 0 до 99%';
    }
  }
  
  // Единица измерения
  if (!product.value.tovarUnit) {
    errors.tovarUnit = 'Выберите единицу измерения';
  }
  
  // Изображение для нового товара
  if (!isEditMode.value && !product.value.tovarImage) {
    errors.tovarImage = 'Изображение обязательно для нового товара';
  }
  
  validationErrors.value = errors;
  return Object.keys(errors).length === 0;
};

// ============================================
// ОСНОВНЫЕ ФУНКЦИИ
// ============================================

const fetchData = async () => {
  try {
    isLoading.value = true
    
    const results = await Promise.allSettled([
      axios.get('http://localhost:5122/api/Tovar/GetManufacturers').catch(e => ({ data: [] })),
      axios.get('http://localhost:5122/api/Tovar/GetSuppliers').catch(e => ({ data: [] })),
      axios.get('http://localhost:5122/api/Tovar/GetCategories').catch(e => ({ data: [] }))
    ]);
    
    manufacturers.value = results[0].status === 'fulfilled' ? results[0].value.data : [];
    suppliers.value = results[1].status === 'fulfilled' ? results[1].value.data : [];
    categories.value = results[2].status === 'fulfilled' ? results[2].value.data : [];
    
  } catch (error) {
    console.error('Ошибка при загрузке данных:', error)
    manufacturers.value = [];
    suppliers.value = [];
    categories.value = [];
  } finally {
    isLoading.value = false
  }
};

const fetchProduct = async (id) => {
  try {
    isLoading.value = true
    
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 10000);
    
    const response = await axios.get(`http://localhost:5122/api/Tovar/GetTovarId?id=${id}`, {
      signal: controller.signal
    });
    
    clearTimeout(timeoutId);
    
    const data = response.data;
    
    console.log('Полученные данные товара:', data);
    
    product.value = {
      tovarArticle: data.tovarArticle || '',
      tovarName: data.tovarName || '',
      tovarUnit: data.tovarUnit || '',
      tovarCost: data.tovarCost || 0,
      supplierId: data.supplierId ? data.supplierId.toString() : null,
      manufacturerId: data.manufacturerId ? data.manufacturerId.toString() : null,
      tovarCategoryId: data.tovarCategoryId ? data.tovarCategoryId.toString() : null,
      tovarDiscount: data.tovarDiscount || 0,
      tovarCount: data.tovarCount || 0,
      tovarDescription: data.tovarDescription || '',
      tovarImage: data.tovarImage || ''
    };
    
    // Сохраняем оригинальное имя файла
    originalImageName.value = data.tovarImage || '';
    
    // Устанавливаем превью изображения
    if (product.value.tovarImage) {
      const fileInfo = getImageFromStorage(product.value.tovarImage);
      if (fileInfo && fileInfo.data) {
        imagePreview.value = fileInfo.data;
      } else {
        imagePreview.value = `/resources/${product.value.tovarImage}`;
      }
    }
    
  } catch (error) {
    if (error.name === 'AbortError') {
      console.error('Запрос превысил время ожидания');
      alert('Время загрузки товара истекло. Пожалуйста, попробуйте еще раз.');
    } else {
      console.error('Ошибка при загрузке товара:', error);
      alert('Ошибка при загрузке товара. Пожалуйста, проверьте соединение.');
    }
  } finally {
    isLoading.value = false;
  }
};

// Проверка прав пользователя
const checkUserPermissions = () => {
  const userData = cookies.get('user')
  if (!userData || userData.userRole !== 'Администратор') {
    router.push('/home')
    return false
  }
  isAdmin.value = true
  return true
};

// Сохранение товара
const saveProduct = async () => {
  try {
    if (isSavingImage.value) {
      alert('Пожалуйста, дождитесь окончания загрузки изображения');
      return;
    }
    
    // Валидация
    if (!validateProduct()) {
      // Прокручиваем к первой ошибке
      const firstError = Object.keys(validationErrors.value)[0];
      const element = document.querySelector(`[name="${firstError}"], .form-group:has([v-model="product.${firstError}"])`);
      if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
      return;
    }

    const productData = {
      ...product.value,
      tovarImage: product.value.tovarImage,
      supplierId: product.value.supplierId ? parseInt(product.value.supplierId) : null,
      manufacturerId: product.value.manufacturerId ? parseInt(product.value.manufacturerId) : null,
      tovarCategoryId: product.value.tovarCategoryId ? parseInt(product.value.tovarCategoryId) : null,
      tovarCost: parseFloat(product.value.tovarCost),
      tovarDiscount: parseFloat(product.value.tovarDiscount || 0),
      tovarCount: parseInt(product.value.tovarCount)
    }

    console.log('Отправляемые данные:', productData);

    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 30000);

    try {
      if (isEditMode.value) {
        await axios.put('http://localhost:5122/api/Tovar/Update', productData, {
          signal: controller.signal
        });
        alert('Товар успешно обновлен!');
      } else {
        await axios.post('http://localhost:5122/api/Tovar/Add', productData, {
          signal: controller.signal
        });
        alert('Товар успешно добавлен!');
      }
      
      clearTimeout(timeoutId);
      router.push('/home');
      
    } catch (error) {
      clearTimeout(timeoutId);
      
      if (error.name === 'AbortError') {
        alert('Время сохранения истекло. Пожалуйста, попробуйте еще раз.');
      } else if (error.response) {
        console.error('Ошибка сервера:', error.response.data);
        alert(error.response.data || 'Ошибка при сохранении товара');
      } else {
        console.error('Ошибка сети:', error.message);
        alert('Ошибка сети. Проверьте подключение к серверу.');
      }
    }
    
  } catch (error) {
    console.error('Непредвиденная ошибка:', error);
    alert('Произошла непредвиденная ошибка');
  }
};

// Отмена
const cancel = () => {
  router.push('/home');
};

onMounted(async () => {
  if (!checkUserPermissions()) {
    return
  }
  
  try {
    await fetchData();
    
    if (route.params.id) {
      isEditMode.value = true;
      productId.value = route.params.id;
      await fetchProduct(productId.value);
    }
  } catch (error) {
    console.error('Ошибка при инициализации:', error);
  }
});
</script>

<template>
  <div class="product-form-wrapper">
    <div class="product-form">
      <div class="form-header">
        <h1>{{ isEditMode ? 'Редактирование товара' : 'Добавление нового товара' }}</h1>
      </div>
      
      <div v-if="isLoading" class="loading">
        <div class="spinner"></div>
        <p>Загрузка данных...</p>
      </div>
      
      <div v-else class="form-content">
        <div class="form-grid">
          <div class="form-column">
            <div class="form-group" :class="{ 'has-error': validationErrors.tovarArticle }">
              <label>Артикул *</label>
              <input 
                v-model="product.tovarArticle" 
                type="text" 
                :disabled="isEditMode"
                placeholder="Введите артикул товара"
                class="form-input"
                :class="{ 'loading-input': isSavingImage, 'error-input': validationErrors.tovarArticle }"
                name="tovarArticle"
              />
              <span v-if="validationErrors.tovarArticle" class="error-message">{{ validationErrors.tovarArticle }}</span>
            </div>
            
            <div class="form-group" :class="{ 'has-error': validationErrors.tovarName }">
              <label>Название товара *</label>
              <input 
                v-model="product.tovarName" 
                type="text" 
                placeholder="Введите название товара"
                class="form-input"
                :class="{ 'loading-input': isSavingImage, 'error-input': validationErrors.tovarName }"
                name="tovarName"
              />
              <span v-if="validationErrors.tovarName" class="error-message">{{ validationErrors.tovarName }}</span>
            </div>
            
            <div class="form-group">
              <label>Описание *</label>
              <textarea 
                v-model="product.tovarDescription" 
                placeholder="Введите описание товара"
                class="form-textarea"
                rows="4"
                :class="{ 'loading-input': isSavingImage }"
              ></textarea>
            </div>
            
            <div class="form-group" :class="{ 'has-error': validationErrors.tovarImage }">
              <label>Изображение товара *</label>
              
              <div class="image-upload-container">
                <!-- Превью изображения -->
                <div v-if="imagePreview" class="image-preview">
                  <img :src="imagePreview" alt="Превью изображения" />
                  <div v-if="isSavingImage" class="image-saving-overlay">
                    <div class="saving-spinner"></div>
                  </div>
                  <button 
                    @click="clearImage" 
                    type="button" 
                    class="remove-image-btn" 
                    title="Удалить изображение"
                    :disabled="isSavingImage"
                  >
                    ×
                  </button>
                </div>
                
                <!-- Область для загрузки -->
                <div 
                  v-else 
                  class="upload-area" 
                  @click="$refs.fileInput.click()"
                  :class="{ 'uploading': isSavingImage, 'error-area': validationErrors.tovarImage }"
                >
                  <div v-if="isSavingImage" class="uploading-overlay">
                    <div class="uploading-spinner"></div>
                    <p>Загрузка...</p>
                  </div>
                  <div v-else>
                    <div class="upload-icon">
                      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
                        <polyline points="17 8 12 3 7 8"></polyline>
                        <line x1="12" y1="3" x2="12" y2="15"></line>
                      </svg>
                    </div>
                    <p>Нажмите для выбора изображения</p>
                    <small>Требования: 300x200 пикселей, макс. 5MB</small>
                    <small>Поддерживаются: JPG, PNG, GIF, WebP</small>
                  </div>
                </div>
                
                <!-- Скрытый input для файлов -->
                <input 
                  ref="fileInput"
                  type="file" 
                  accept="image/*"
                  @change="handleFileSelect"
                  style="display: none"
                  :disabled="isSavingImage"
                />
              </div>
              
              <span v-if="validationErrors.tovarImage" class="error-message">{{ validationErrors.tovarImage }}</span>
              <span v-if="imageValidationError" class="error-message">{{ imageValidationError }}</span>
              
            </div>
          </div>
          
          <div class="form-column">
            <div class="form-group">
              <label>Производитель</label>
              <select v-model="product.manufacturerId" class="form-select" :disabled="isSavingImage">
                <option value="">Выберите производителя</option>
                <option 
                  v-for="manufacturer in manufacturers" 
                  :key="manufacturer.manufacturerId"
                  :value="manufacturer.manufacturerId ? manufacturer.manufacturerId.toString() : ''"
                >
                  {{ manufacturer.manufacturerName }}
                </option>
              </select>
            </div>
            
            <div class="form-group">
              <label>Категория</label>
              <select v-model="product.tovarCategoryId" class="form-select" :disabled="isSavingImage">
                <option value="">Выберите категорию</option>
                <option 
                  v-for="category in categories" 
                  :key="category.tovarCategoryId"
                  :value="category.tovarCategoryId ? category.tovarCategoryId.toString() : ''"
                >
                  {{ category.tovarCategoryName }}
                </option>
              </select>
            </div>
            
            <div class="form-group">
              <label>Поставщик</label>
              <select v-model="product.supplierId" class="form-select" :disabled="isSavingImage">
                <option value="">Выберите поставщика</option>
                <option 
                  v-for="supplier in suppliers" 
                  :key="supplier.supplierId"
                  :value="supplier.supplierId ? supplier.supplierId.toString() : ''"
                >
                  {{ supplier.supplierName }}
                </option>
              </select>
            </div>
            
            <div class="form-row">
              <div class="form-group" :class="{ 'has-error': validationErrors.tovarCost }">
                <label>Стоимость *</label>
                <input 
                  v-model.number="product.tovarCost" 
                  type="number" 
                  step="0.01"
                  min="0"
                  placeholder="0.00"
                  class="form-input"
                  :class="{ 'loading-input': isSavingImage, 'error-input': validationErrors.tovarCost }"
                  :disabled="isSavingImage"
                  name="tovarCost"
                />
                <span v-if="validationErrors.tovarCost" class="error-message">{{ validationErrors.tovarCost }}</span>
              </div>
              
              <div class="form-group" :class="{ 'has-error': validationErrors.tovarDiscount }">
                <label>Скидка (%)</label>
                <input 
                  v-model.number="product.tovarDiscount" 
                  type="number" 
                  min="0"
                  max="100"
                  placeholder="0"
                  class="form-input"
                  :class="{ 'loading-input': isSavingImage, 'error-input': validationErrors.tovarDiscount }"
                  :disabled="isSavingImage"
                  name="tovarDiscount"
                />
                <span v-if="validationErrors.tovarDiscount" class="error-message">{{ validationErrors.tovarDiscount }}</span>
              </div>
            </div>
            
            <div class="form-row">
              <div class="form-group" :class="{ 'has-error': validationErrors.tovarCount }">
                <label>Количество *</label>
                <input 
                  v-model.number="product.tovarCount" 
                  type="number" 
                  min="0"
                  placeholder="0"
                  class="form-input"
                  :class="{ 'loading-input': isSavingImage, 'error-input': validationErrors.tovarCount }"
                  :disabled="isSavingImage"
                  name="tovarCount"
                />
                <span v-if="validationErrors.tovarCount" class="error-message">{{ validationErrors.tovarCount }}</span>
              </div>
              
              <div class="form-group" :class="{ 'has-error': validationErrors.tovarUnit }">
                <label>Единица измерения *</label>
                <select v-model="product.tovarUnit" class="form-select" :class="{ 'error-input': validationErrors.tovarUnit }" :disabled="isSavingImage">
                  <option value="">Выберите единицу</option>
                  <option value="шт.">шт.</option>
                </select>
                <span v-if="validationErrors.tovarUnit" class="error-message">{{ validationErrors.tovarUnit }}</span>
              </div>
            </div>
          </div>
        </div>
        
        <div class="form-actions">
          
          <div class="action-buttons">
            <button @click="cancel" class="btn btn-secondary" :disabled="isSavingImage">
              Отмена
            </button>
            
            <button @click="saveProduct" class="btn btn-primary" :disabled="isSavingImage">
              <span v-if="isSavingImage">Обработка...</span>
              <span v-else>{{ isEditMode ? 'Сохранить изменения' : 'Добавить товар' }}</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Добавляем новые стили для валидации */
.has-error .form-input,
.has-error .form-select,
.has-error .form-textarea {
  border-color: #f44336;
}

.has-error .form-input:focus,
.has-error .form-select:focus,
.has-error .form-textarea:focus {
  border-color: #f44336;
  box-shadow: 0 0 0 3px rgba(244, 67, 54, 0.1);
}

.error-input {
  border-color: #f44336 !important;
}

.error-message {
  color: #f44336;
  font-size: 12px;
  margin-top: 4px;
  display: block;
}

.error-area {
  border-color: #f44336 !important;
  background-color: #fff5f5;
}

.success-message {
  color: #4CAF50;
}

/* Остальные стили остаются прежними */
.product-form-wrapper {
  min-height: 100vh;
  background: #f9f9f9;
  padding: 80px 20px 40px;
}

.product-form {
  max-width: 900px;
  margin: 0 auto;
  background: white;
  border-radius: 16px;
  box-shadow: 0 8px 30px rgba(255, 126, 179, 0.1);
  overflow: hidden;
}

.form-header {
  padding: 30px 40px;
  background: linear-gradient(135deg, #ff4081 0%, #ff7eb3 100%);
  color: white;
}

.form-header h1 {
  font-size: 24px;
  font-weight: 600;
  margin: 0;
}

.form-content {
  padding: 40px;
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

.loading .spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #ff4081;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 40px;
}

@media (max-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
}

.form-column {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 14px;
  font-weight: 600;
  color: #333;
  display: flex;
  align-items: center;
  gap: 4px;
}

.form-group label:after {
  content: '*';
  color: #ff4081;
  display: inline;
}

.form-group label:not(:has(+ input:required)):after {
  content: '';
}

.form-input, .form-select, .form-textarea {
  padding: 12px 16px;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-size: 14px;
  transition: all 0.3s ease;
  background: white;
  font-family: inherit;
}

.form-input:focus, .form-select:focus, .form-textarea:focus {
  outline: none;
  border-color: #ff7eb3;
  box-shadow: 0 0 0 3px rgba(255, 126, 179, 0.1);
}

.form-input[disabled] {
  background: #f5f5f5;
  color: #999;
  cursor: not-allowed;
}

.form-textarea {
  resize: vertical;
  min-height: 100px;
}

.form-hint {
  font-size: 12px;
  color: #666;
  margin-top: 4px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

@media (max-width: 480px) {
  .form-row {
    grid-template-columns: 1fr;
    gap: 20px;
  }
}

/* Стили для загрузки изображений */
.image-upload-container {
  margin-top: 8px;
  position: relative;
}

.upload-area {
  border: 2px dashed #e0e0e0;
  border-radius: 8px;
  padding: 30px 20px;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  background: #f9f9f9;
  position: relative;
}

.upload-area:hover:not(.uploading) {
  border-color: #ff7eb3;
  background: #fff5f7;
}

.upload-icon {
  margin-bottom: 10px;
  color: #999;
}

.upload-area p {
  margin: 0 0 5px 0;
  color: #666;
  font-weight: 500;
}

.upload-area small {
  color: #999;
}

.image-preview {
  position: relative;
  width: 300px;
  height: 200px;
  border-radius: 8px;
  overflow: hidden;
  border: 2px solid #e0e0e0;
}

.image-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.remove-image-btn {
  position: absolute;
  top: 8px;
  right: 8px;
  width: 28px;
  height: 28px;
  background: rgba(255, 64, 129, 0.9);
  color: white;
  border: none;
  border-radius: 50%;
  font-size: 18px;
  line-height: 1;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  z-index: 2;
}

.remove-image-btn:hover:not(:disabled) {
  background: rgba(255, 64, 129, 1);
  transform: scale(1.1);
}

.remove-image-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.image-info {
  margin-top: 10px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.loading-input {
  opacity: 0.7;
  cursor: wait;
}

.uploading {
  opacity: 0.5;
  cursor: wait;
}

.uploading-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.8);
}

.uploading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid #f3f3f3;
  border-top: 2px solid #ff4081;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 10px;
}

.image-saving-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
}

.saving-spinner {
  width: 30px;
  height: 30px;
  border: 3px solid rgba(255, 255, 255, 0.3);
  border-top: 3px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.form-actions {
  display: flex;
  flex-direction: column;
  gap: 20px;
  margin-top: 40px;
  padding-top: 30px;
  border-top: 1px solid #eee;
}

.delete-section {
  padding: 15px;
  background: #fff5f5;
  border-radius: 8px;
  border: 1px solid #ffcccc;
}

.delete-warning {
  margin-top: 10px;
  padding: 10px;
  background: #ffe6e6;
  border-radius: 6px;
  color: #d32f2f;
  font-size: 13px;
  line-height: 1.4;
}

.delete-warning p {
  margin: 0;
}

.action-buttons {
  display: flex;
  justify-content: flex-end;
  gap: 20px;
}

@media (max-width: 768px) {
  .action-buttons {
    flex-direction: column;
    align-items: stretch;
  }
}

.btn {
  padding: 12px 30px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  font-family: inherit;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none !important;
  box-shadow: none !important;
}

.btn-primary {
  background: linear-gradient(135deg, #ff4081 0%, #ff7eb3 100%);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(255, 64, 129, 0.3);
}

.btn-primary:active:not(:disabled) {
  transform: translateY(0);
}

.btn-secondary {
  background: #f5f5f5;
  color: #666;
  border: 2px solid #e0e0e0;
}

.btn-secondary:hover:not(:disabled) {
  background: #e0e0e0;
}

.btn-danger {
  background: linear-gradient(135deg, #f44336 0%, #e53935 100%);
  color: white;
  border: none;
  width: 100%;
}

.btn-danger:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(244, 67, 54, 0.3);
  background: linear-gradient(135deg, #e53935 0%, #d32f2f 100%);
}

.btn-danger:active:not(:disabled) {
  transform: translateY(0);
}

@media (max-width: 768px) {
  .product-form-wrapper {
    padding: 70px 15px 30px;
  }
  
  .form-header {
    padding: 25px 30px;
  }
  
  .form-content {
    padding: 30px 25px;
  }
  
  .btn {
    padding: 14px 20px;
    width: 100%;
  }
  
  .image-preview {
    width: 250px;
    height: 167px;
  }
}
</style>