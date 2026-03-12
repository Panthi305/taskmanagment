import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { User, LoginRequest } from '../models/user';

/*
 * ============================================================================
 * AUTHENTICATION SERVICE - ANGULAR SERVICE FOR USER AUTHENTICATION
 * ============================================================================
 * 
 * ANGULAR SERVICES EXPLANATION:
 * ----------------------------
 * Services in Angular are singleton classes that provide:
 * - Shared data and state across components
 * - Business logic separate from UI components
 * - API communication using HttpClient
 * - Reusable functionality
 * 
 * @Injectable({ providedIn: 'root' }) makes this service:
 * - Available application-wide (singleton)
 * - Automatically injected where needed
 * - No need to add to providers array
 * 
 * REACTIVE PROGRAMMING WITH RXJS:
 * ------------------------------
 * - Observable: Stream of data that can be subscribed to
 * - BehaviorSubject: Observable that holds current value
 * - Subscribers get notified when value changes
 * - Components can react to authentication state changes
 * 
 * HTTP CLIENT USAGE:
 * -----------------
 * Angular's HttpClient provides:
 * - Type-safe HTTP requests
 * - Observable-based async operations
 * - Automatic JSON parsing
 * - Interceptor support
 * - Error handling
 * 
 * withCredentials: true enables:
 * - Sending cookies with requests
 * - Required for session-based authentication
 * - Browser includes session cookie automatically
 * 
 * SPA (SINGLE PAGE APPLICATION) CONCEPT:
 * -------------------------------------
 * This Angular app is an SPA, meaning:
 * - Entire app loads once (index.html + JavaScript bundles)
 * - Navigation happens client-side (no page reloads)
 * - Only data is fetched from server (via API calls)
 * - Fast, responsive user experience
 * - State is maintained in memory (services like this one)
 * 
 * When user refreshes page:
 * - App reloads, state is lost
 * - checkSession() restores state from server session
 * - User remains logged in if session is valid
 */

@Injectable({
    providedIn: 'root'  // Singleton service available app-wide
})
export class AuthService {
    // API base URL - points to ASP.NET Core backend
    private apiUrl = 'http://localhost:5150/api/auth';

    // BehaviorSubject holds current user state
    // null = not logged in, User object = logged in
    // Components subscribe to currentUser$ to react to auth state changes
    private currentUserSubject = new BehaviorSubject<User | null>(null);
    public currentUser$ = this.currentUserSubject.asObservable();

    constructor(private http: HttpClient) {
        // Load user from localStorage first (for immediate state restoration)
        this.loadUserFromStorage();
        // Then check session with backend (for validation)
        this.checkSession();
    }

    /*
     * ========================================================================
     * LOGIN - AUTHENTICATE USER
     * ========================================================================
     * 
     * HTTP REQUEST-RESPONSE LIFECYCLE:
     * -------------------------------
     * 1. Component calls authService.login(credentials)
     * 2. HttpClient sends POST request to backend
     * 3. Request includes credentials in body as JSON
     * 4. withCredentials: true allows cookies to be sent/received
     * 5. Backend validates credentials
     * 6. Backend creates session and sends session cookie
     * 7. Browser stores cookie automatically
     * 8. Response returns user data as Observable
     * 9. tap() operator updates currentUserSubject
     * 10. All subscribers get notified of new user state
     * 11. Component subscribes and handles success/error
     * 
     * OBSERVABLE PATTERN:
     * ------------------
     * - Returns Observable<User> (not User directly)
     * - Component must subscribe to get the value
     * - Allows async operation without blocking UI
     * - Can be cancelled if component is destroyed
     */
    login(credentials: LoginRequest): Observable<User> {
        return this.http.post<User>(`${this.apiUrl}/login`, credentials, { withCredentials: true })
            .pipe(
                tap(user => {
                    this.currentUserSubject.next(user);  // Update auth state
                    this.saveUserToStorage(user);  // Persist to localStorage
                })
            );
    }

    /*
     * ========================================================================
     * LOGOUT - END USER SESSION
     * ========================================================================
     * 
     * Sends logout request to backend and clears local auth state.
     */
    logout(): Observable<any> {
        return this.http.post(`${this.apiUrl}/logout`, {}, { withCredentials: true })
            .pipe(
                tap(() => {
                    this.currentUserSubject.next(null);  // Clear auth state
                    this.clearUserFromStorage();  // Clear localStorage
                })
            );
    }

    /*
     * ========================================================================
     * CHECK SESSION - RESTORE AUTH STATE
     * ========================================================================
     * 
     * Called on app initialization to check if user has valid session.
     * If session exists, restores user state.
     * If session expired, sets state to null.
     * 
     * This enables persistent login across page refreshes.
     */
    checkSession(): void {
        this.http.get<User>(`${this.apiUrl}/session`, { withCredentials: true })
            .subscribe({
                next: (user) => {
                    this.currentUserSubject.next(user);
                    this.saveUserToStorage(user);  // Update localStorage with fresh data
                },
                error: () => {
                    this.currentUserSubject.next(null);
                    this.clearUserFromStorage();  // Clear invalid session
                }
            });
    }

    /*
     * ========================================================================
     * LOCAL STORAGE MANAGEMENT - PERSIST AUTH STATE
     * ========================================================================
     */

    // Save user to localStorage for persistence across page refreshes
    private saveUserToStorage(user: User): void {
        localStorage.setItem('user', JSON.stringify(user));
    }

    // Load user from localStorage on app initialization
    private loadUserFromStorage(): void {
        const storedUser = localStorage.getItem('user');
        if (storedUser) {
            try {
                const user = JSON.parse(storedUser);
                this.currentUserSubject.next(user);
            } catch (error) {
                // Invalid JSON, clear storage
                this.clearUserFromStorage();
            }
        }
    }

    // Clear user from localStorage on logout
    private clearUserFromStorage(): void {
        localStorage.removeItem('user');
    }

    /*
     * ========================================================================
     * HELPER METHODS - AUTH STATE CHECKS
     * ========================================================================
     */

    // Get current user synchronously (from BehaviorSubject's current value)
    getCurrentUser(): User | null {
        return this.currentUserSubject.value;
    }

    // Check if user is authenticated
    isAuthenticated(): boolean {
        return this.currentUserSubject.value !== null;
    }

    // Check if user has specific role (for role-based UI rendering)
    hasRole(role: string): boolean {
        const user = this.currentUserSubject.value;
        return user !== null && user.role === role;
    }
}
