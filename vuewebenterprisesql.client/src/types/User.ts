export interface User {
    id: string;
    azureAdUserId: string;
    email: string;
    fullName: string;
    lastLoginAt: string; // Dates usually come as strings from JSON
}
