<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();

// Поля для регистрации согласно модели User
const userSurname = ref('');
const userName = ref('');
const userLastname = ref('');
const userLogin = ref(''); // email
const userPass = ref('');
const confirmPassword = ref('');

async function register() {
  try {
    if (userSurname.value == '' || userName.value == '' || userLastname.value == '' || 
        userLogin.value == '' || userPass.value == '' || confirmPassword.value == '') {
      alert("Все поля должны быть заполнены!");
      return;
    }

    if (userPass.value !== confirmPassword.value) {
      alert("Пароли не совпадают!");
      return;
    }

    if (userPass.value.length < 6) {
      alert("Пароль должен содержать минимум 6 символов!");
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(userLogin.value)) {
      alert("Введите корректный email адрес!");
      return;
    }

    const userData = {
      userSurname: userSurname.value,
      userName: userName.value,
      userLastname: userLastname.value,
      userLogin: userLogin.value,
      userPass: userPass.value
    };
    
    const response = await axios.post('http://localhost:5122/api/User/Register', userData);
    
    if (response.data) {
      alert("Регистрация прошла успешно! Теперь вы можете войти.");
      router.push('/');
    }
  } catch(error) {
    console.error('Ошибка регистрации:', error);
    if (error.response) {
      alert('Ошибка: ' + (error.response.data || 'Ошибка сервера'));
    } else if (error.request) {
      alert('Ошибка: Сервер не отвечает');
    } else {
      alert('Ошибка: ' + error.message);
    }
  }
}

const goToLogin = () => {
  router.push('/');
}
</script>

<template>
  <main class="main">
    <div class="register-container">
      <div class="auth-header">
        <h1>Регистрация</h1>
      </div>
      
      <form @submit.prevent="register" class="auth-form">
        <div class="input-form">
          <label class="input-label">Фамилия</label>
          <input 
            type="text" 
            v-model="userSurname" 
            placeholder="Введите фамилию" 
            required 
            class="form-input"
          >
        </div>

        <div class="input-form">
          <label class="input-label">Имя</label>
          <input 
            type="text" 
            v-model="userName" 
            placeholder="Введите имя" 
            required 
            class="form-input"
          >
        </div>

        <div class="input-form">
          <label class="input-label">Отчество</label>
          <input 
            type="text" 
            v-model="userLastname" 
            placeholder="Введите отчество" 
            required 
            class="form-input"
          >
        </div>

        <div class="input-form">
          <label class="input-label">Email (логин)</label>
          <input 
            type="email" 
            v-model="userLogin" 
            placeholder="Введите email" 
            required 
            class="form-input"
          >
        </div>
        
        <div class="input-form">
          <label class="input-label">Пароль</label>
          <input 
            type="password" 
            v-model="userPass" 
            placeholder="Минимум 6 символов"
            required 
            class="form-input"
          >
        </div>

        <div class="input-form">
          <label class="input-label">Подтверждение пароля</label>
          <input 
            type="password" 
            v-model="confirmPassword" 
            placeholder="Повторите пароль"
            required 
            class="form-input"
          >
        </div>
        
        <button type="submit" class="register-btn">
          Зарегистрироваться
        </button>
        
        <div class="login-section">
          <p class="login-text">Уже есть аккаунт?</p>
          <button @click="goToLogin" type="button" class="login-link-btn">
            Войти
          </button>
        </div>
      </form>
    </div>
  </main>
</template>

<style scoped>
.main {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 100px);
  padding: 30px 20px;
  background: white;
  margin-top: 50px;
}

.register-container {
  display: flex;
  height: auto;
  width: 90vh;
  max-width: 500px;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  padding: 40px;
  background-color: #ffffff;
  border-radius: 16px;
  box-shadow: 0 8px 32px rgba(255, 184, 217, 0.15);
  border: 1px solid #ffe6f0;
}

.auth-header h1 {
  font-size: 32px;
  font-weight: 600;
  margin-bottom: 30px;
  color: #d14a7c;
  text-align: center;
  background: linear-gradient(135deg, #ff7eb3 0%, #ffb8d9 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.auth-form {
  width: 100%;
  margin: 20px 0;
}

.input-form {
  display: flex;
  flex-direction: column;
  margin-bottom: 20px;
}

.input-label {
  font-size: 16px;
  font-weight: 500;
  margin-bottom: 8px;
  color: #e6729b;
}

.form-input {
  background-color: #fff9fc;
  border-radius: 10px;
  border: 2px solid #ffd6e7;
  font-size: 16px;
  padding: 12px 18px;
  width: 100%;
  box-sizing: border-box;
  color: #333;
  transition: all 0.3s ease;
}

.form-input:focus {
  outline: none;
  border-color: #ff7eb3;
  background-color: #fff;
  box-shadow: 0 0 0 3px rgba(255, 126, 179, 0.1);
}

.form-input::placeholder {
  color: #ffb8d9;
  font-size: 14px;
}

.register-btn {
  background: linear-gradient(135deg, #ff7eb3 0%, #ffb8d9 100%);
  color: white;
  border-radius: 10px;
  border: none;
  font-size: 16px;
  font-weight: 600;
  padding: 16px 30px;
  width: 100%;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(255, 126, 179, 0.2);
  margin-top: 10px;
}

.register-btn:hover {
  background: linear-gradient(135deg, #ff6ba3 0%, #ffa8d1 100%);
  box-shadow: 0 6px 20px rgba(255, 126, 179, 0.3);
  transform: translateY(-2px);
}

.register-btn:active {
  transform: translateY(0);
}

.login-section {
  margin-top: 30px;
  text-align: center;
  width: 100%;
  padding-top: 20px;
  border-top: 1px solid #ffe6f0;
}

.login-text {
  color: #ff7eb3;
  margin-bottom: 15px;
  font-size: 14px;
  font-weight: 500;
}

.login-link-btn {
  background: transparent;
  color: #ff7eb3;
  border: 2px solid #ff7eb3;
  border-radius: 10px;
  font-size: 16px;
  font-weight: 600;
  padding: 14px 30px;
  width: 100%;
  cursor: pointer;
  transition: all 0.3s ease;
}

.login-link-btn:hover {
  background: linear-gradient(135deg, #ff7eb3 0%, #ffb8d9 100%);
  color: white;
  border-color: #ff7eb3;
  box-shadow: 0 4px 15px rgba(255, 126, 179, 0.2);
}
</style>