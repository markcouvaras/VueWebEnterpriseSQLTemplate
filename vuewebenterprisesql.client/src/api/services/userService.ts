import apiClient from '@/api/axios';
import type { User } from '@/types/User'; // Assuming you have a type defined

export const userService = {
  getDummy: async (): Promise<User[]> => {
    const response = await apiClient.get<User[]>('/users/dummy');
    return response.data;
  },

  getAll: async (): Promise<User[]> => {
    const response = await apiClient.get<User[]>('/users');
    return response.data;
  },

  getCurrentUser: async (): Promise<User> => {
    const response = await apiClient.get<User>('/users/me');
    return response.data;
  },

  getById: async (id: string): Promise<User> => {
    const response = await apiClient.get<User>(`/users/${id}`);
    return response.data;
  },

  // Example of a POST
  create: async (userData: Partial<User>): Promise<User> => {
    const response = await apiClient.post<User>('/users', userData);
    return response.data;
  }
};
