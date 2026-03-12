import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User, CreateUserRequest } from '../models/user';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private apiUrl = 'http://localhost:5150/api/user';

    constructor(private http: HttpClient) { }

    getUsers(): Observable<User[]> {
        return this.http.get<User[]>(this.apiUrl, { withCredentials: true });
    }

    getAssignableUsers(): Observable<User[]> {
        return this.http.get<User[]>(`${this.apiUrl}/assignable`, { withCredentials: true });
    }

    createUser(user: CreateUserRequest): Observable<User> {
        return this.http.post<User>(this.apiUrl, user, { withCredentials: true });
    }

    deleteUser(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`, { withCredentials: true });
    }
}
