import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: process.env.REACT_APP_API_URL
});

// Request Interceptor
axiosInstance.interceptors.request.use(
  config => {
    // Modify the request config before sending the request
    // For example, add an authentication token
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    // Handle request error
    return Promise.reject(error);
  }
);

// Response Interceptor
axiosInstance.interceptors.response.use(
  response => {
    // Any status code within the range of 2xx causes this function to trigger
    return response;
  },
  error => {
    // Any status code outside the range of 2xx causes this function to trigger
    // Handle response error
    if (error.response.status === 401) {
      // Handle unauthorized errors, e.g., redirect to login page
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;