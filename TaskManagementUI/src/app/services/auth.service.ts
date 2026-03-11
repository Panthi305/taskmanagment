import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { User, LoginRequest } from '../models/user';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = 'http://localhost:5150/api/auth';
    private currentUserSubject = new BehaviorSubject<User | null>(null);
    public currentUser$ = this.currentUserSubject.asObservable();

    constructor(private http: HttpClient) {
        this.checkSession();
    }

    login(credentials: LoginRequest): Observable<User> {
        return this.http.post<User>(`${this.apiUrl}/login`, credentials, { withCredentials: true })
            .pipe(
                tap(user => this.currentUserSubject.next(user))
            );
    }

    logout(): Observable<any> {
        return this.http.post(`${this.apiUrl}/logout`, {}, { withCredentials: true })
            .pipe(
                tap(() => this.currentUserSubject.next(null))
            );
    }

    checkSession(): void {
        this.http.get<User>(`${this.apiUrl}/session`, { withCredentials: true })
            .subscribe({
                next: (user) => this.currentUserSubject.next(user),
                error: () => this.currentUserSubject.next(null)
            });
    }

    getCurrentUser(): User | null {
        return this.currentUserSubject.value;
    }

    isAuthenticated(): boolean {
        return this.currentUserSubject.value !== null;
    }

    hasRole(role: string): boolean {
        const user = this.currentUserSubject.value;
        return user !== null && user.role === role;
    }
}
