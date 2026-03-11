export interface User {
    id: number;
    name: string;
    email: string;
    role: string;
    createdAt?: Date;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface CreateUserRequest {
    name: string;
    email: string;
    password: string;
    role: string;
}
