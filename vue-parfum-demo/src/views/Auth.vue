<script setup>
import { ref } from 'vue';
import { useCookies } from 'vue3-cookies';
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();
const { cookies } = useCookies();

const email = ref('');
const password = ref('');

async function auth() { 
  try {
    if(password.value == '' || email.value == ''){
      alert("Поля не должны быть пустыми!")
      return
    }
    
    const response = await axios.post('http://localhost:5122/api/User/Login', 
      null,
      {
        params: {
          login: email.value,
          pass: password.value
        }
      }
    );
    
    if (response.data) {
      alert("Вы успешно вошли в профиль!");
      console.log('Пользователь:', response.data);
      
      cookies.set('user', response.data);
      
      cookies.remove('guest');
      
      router.push('/home');
    } else {
      alert('Ошибка: Неверные данные пользователя');
    }
  }
  catch(error) {
    console.error('Ошибка авторизации:', error);
    if (error.response) {
      const errorMessage = error.response.data;
      alert('Ошибка: ' + errorMessage);
    } else if (error.request) {
      alert('Ошибка: Сервер не отвечает');
    } else {
      alert('Ошибка: ' + error.message);
    }
  }
}

// Функция для входа как гость
const loginAsGuest = () => {
  cookies.set('guest', 'true', '1h');
  cookies.remove('user');
  router.push('/home');
}

// Функция для перехода на страницу регистрации
const goToRegister = () => {
  router.push('/register');
}
</script>

<template>
  <main class="main">
    <div class="login-container">
      <div class="auth-header">
        <h1>Вход в аккаунт</h1>
      </div>
      
      <form @submit.prevent="auth" class="auth-form">
        <div class="input-form">
          <label class="input-label">Логин или Email</label>
          <input 
            type="text" 
            v-model="email" 
            placeholder="Введите логин или email" 
            required 
            class="form-input"
          >
        </div>
        
        <div class="input-form">
          <label class="input-label">Пароль</label>
          <input 
            type="password" 
            v-model="password" 
            placeholder="Введите пароль"
            required 
            class="form-input"
          >
        </div>
        
        <button type="submit" class="auth-btn">
          Войти
        </button>
        
        <div class="guest-section">
          <p class="guest-text">Или</p>
          <button @click="loginAsGuest" type="button" class="guest-btn">
            Войти как гость
          </button>
        </div>

        <div class="register-section">
          <p class="register-text">Нет аккаунта?</p>
          <button @click="goToRegister" type="button" class="register-btn">
            Зарегистрироваться
          </button>
        </div>
      </form>
    </div>
  </main>
</template>

<style scoped>
.main {
  display: flex;
  padding: 30px 20px;
  background: white;
  margin-top: 50px;
}

.login-container {
  display: flex;
  height: auto;
  min-height: 65vh;
  width: 90vh;
  max-width: 500px;
  justify-content: center;
  align-items: center;
  flex-direction: column;
  align-items: center;
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
  margin-bottom: 24px;
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
  padding: 14px 18px;
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

.auth-btn {
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
}

.auth-btn:hover {
  background: linear-gradient(135deg, #ff6ba3 0%, #ffa8d1 100%);
  box-shadow: 0 6px 20px rgba(255, 126, 179, 0.3);
  transform: translateY(-2px);
}

.auth-btn:active {
  transform: translateY(0);
}

.guest-section {
  margin-top: 30px;
  text-align: center;
  width: 100%;
  padding-top: 20px;
  border-top: 1px solid #ffe6f0;
}

.guest-text {
  color: #ff7eb3;
  margin-bottom: 15px;
  font-size: 14px;
  font-weight: 500;
}

.guest-btn {
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

.guest-btn:hover {
  background: linear-gradient(135deg, #ff7eb3 0%, #ffb8d9 100%);
  color: white;
  border-color: #ff7eb3;
  box-shadow: 0 4px 15px rgba(255, 126, 179, 0.2);
}

.register-section {
  margin-top: 20px;
  text-align: center;
  width: 100%;
}

.register-text {
  color: #ff7eb3;
  margin-bottom: 15px;
  font-size: 14px;
  font-weight: 500;
}

.register-btn {
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

.register-btn:hover {
  background: linear-gradient(135deg, #ff7eb3 0%, #ffb8d9 100%);
  color: white;
  border-color: #ff7eb3;
  box-shadow: 0 4px 15px rgba(255, 126, 179, 0.2);
}
</style>