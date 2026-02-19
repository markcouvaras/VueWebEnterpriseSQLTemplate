import { ref } from 'vue';
import { userService } from '@/api/services/userService';
import type { User } from '@/types/User';

export function useUsers() {
  const users = ref<User[]>([]);
  const currentUser = ref<User | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  // Function to fetch dummy users
  const fetchDummyUsers = async () => {
    loading.value = true;
    error.value = null;
    try {
      users.value = await userService.getDummy();
    } catch (err: any) {
      error.value = 'Failed to load dummy users';
      console.error(err);
    } finally {
      loading.value = false;
    }
  }

  // Function to fetch all users
  const fetchUsers = async () => {
    loading.value = true;
    error.value = null;
    try {
      users.value = await userService.getAll();
    } catch (err: any) {
      error.value = 'Failed to load users';
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  // Function to fetch current user (Shadow User)
  const fetchCurrentUser = async () => {
    loading.value = true;
    try {
      currentUser.value = await userService.getCurrentUser();
    } catch (err: any) {
      error.value = 'Failed to load profile';
    } finally {
      loading.value = false;
    }
  };

  return {
    users,
    currentUser,
    loading,
    error,
    fetchDummyUsers,
    fetchUsers,
    fetchCurrentUser
  };
}
