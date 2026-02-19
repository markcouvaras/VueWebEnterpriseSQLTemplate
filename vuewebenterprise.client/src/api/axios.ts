import axios, { type AxiosInstance, type AxiosRequestConfig, type AxiosResponse } from 'axios';
// import { useAuth } from '@/composables/useAuth'; // We will imagine this handles MSAL

// 1. Create the instance
const apiClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '/api',
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
  timeout: 10000, // 10 seconds
});

// 2. Request Interceptor: Injects the Auth Token
apiClient.interceptors.request.use(
  async (config) => {
    // In a real app, you would get the token from your MSAL instance here
    // const { getAccessToken } = useAuth();
    // const token = await getAccessToken();

    const token = localStorage.getItem('authToken'); // Placeholder until MSAL is set up

    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 3. Response Interceptor: Handles global errors (401, 403, 500)
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    return response;
  },
  (error) => {
    const status = error.response ? error.response.status : null;

    if (status === 401) {
      // Handle Unauthorized (e.g., redirect to login or refresh token)
      console.warn('Unauthorized - Redirecting to login...');
      // router.push('/login');
    } else if (status === 403) {
      console.warn('Forbidden - You do not have permission.');
    } else if (status === 500) {
      console.error('Server Error - Please try again later.');
    }

    return Promise.reject(error);
  }
);

export default apiClient;
