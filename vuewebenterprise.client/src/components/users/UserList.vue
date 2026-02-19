<script setup lang="ts">
import { onMounted } from 'vue';
import { useUsers } from '@/composables/useUsers';

// Use the composable logic
const { users, loading, error, fetchDummyUsers } = useUsers();

// Fetch data automatically when component loads
onMounted(() => {
  fetchDummyUsers();
});
</script>

<template>
  <div class="user-container">
    <h2>User Directory</h2>
    <button @click="fetchDummyUsers" :disabled="loading">
      Refresh Users
    </button>

    <div v-if="loading" class="loading">
      Loading users...
    </div>

    <div v-if="error" class="error">
      {{ error }}
    </div>

    <table v-if="!loading && users.length > 0">
      <thead>
        <tr>
          <th>Name</th>
          <th>Email</th>
          <th>Last Login</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="user in users" :key="user.id">
          <td>{{ user.fullName }}</td>
          <td>{{ user.email }}</td>
          <td>{{ new Date(user.lastLoginAt).toLocaleDateString() }}</td>
        </tr>
      </tbody>
    </table>

    <div v-if="!loading && users.length === 0" class="empty">
      No users found in the database.
    </div>
  </div>
</template>

<style scoped>
.user-container {
  padding: 20px;
  max-width: 800px;
  margin: 0 auto;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

th, td {
  text-align: left;
  padding: 12px;
  border-bottom: 1px solid #ddd;
}

th {
  background-color: #f4f4f4;
  color: #333;
}

.error {
  color: red;
  padding: 10px;
  background-color: #ffe6e6;
  border-radius: 4px;
}

.loading {
  color: #666;
  font-style: italic;
}
</style>
