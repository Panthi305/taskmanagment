import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

/*
 * ============================================================================
 * LOGIN COMPONENT - USER AUTHENTICATION UI
 * ============================================================================
 * 
 * ANGULAR COMPONENT EXPLANATION:
 * -----------------------------
 * Components are the building blocks of Angular applications.
 * Each component consists of:
 * 
 * 1. TypeScript Class (this file):
 *    - Component logic
 *    - Data properties
 *    - Methods for user interactions
 * 
 * 2. HTML Template (login.html):
 *    - User interface structure
 *    - Data binding expressions
 *    - Event handlers
 * 
 * 3. CSS Styles (login.css):
 *    - Component-specific styling
 *    - Scoped to this component only
 * 
 * STANDALONE COMPONENTS:
 * ---------------------
 * standalone: true means:
 * - Component is self-contained
 * - Declares its own dependencies in imports array
 * - No need for NgModule
 * - Modern Angular approach (v14+)
 * 
 * IMPORTS EXPLAINED:
 * -----------------
 * - CommonModule: Provides common directives (*ngIf, *ngFor, etc.)
 * - FormsModule: Enables template-driven forms and ngModel
 * - Router: For navigation between pages
 * - AuthService: Injected service for authentication logic
 * 
 * TEMPLATE-DRIVEN FORMS:
 * ---------------------
 * This component uses template-driven forms (not reactive forms).
 * Features:
 * - Two-way data binding with [(ngModel)]
 * - Simple syntax for basic forms
 * - Form validation in template
 * - Good for simple forms
 * 
 * DEPENDENCY INJECTION:
 * --------------------
 * Services are injected via constructor:
 * - Angular provides instances automatically
 * - No need to create instances manually
 * - Promotes loose coupling and testability
 */

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],  // Required modules for this component
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  // Component properties - bound to template
  email = '';
  password = '';
  errorMessage = '';

  // Constructor injection - Angular provides service instances
  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  /*
   * ==========================================================================
   * ON SUBMIT - HANDLE LOGIN FORM SUBMISSION
   * ==========================================================================
   * 
   * EVENT BINDING:
   * -------------
   * In template: <form (ngSubmit)="onSubmit()">
   * - (ngSubmit) is event binding syntax
   * - Calls this method when form is submitted
   * - Prevents default form submission (no page reload)
   * 
   * TWO-WAY DATA BINDING:
   * --------------------
   * In template: <input [(ngModel)]="email">
   * - [(ngModel)] creates two-way binding
   * - Changes in input update component property
   * - Changes in property update input value
   * - Banana in a box syntax: [( )]
   * 
   * OBSERVABLE SUBSCRIPTION:
   * -----------------------
   * authService.login() returns Observable
   * .subscribe() executes the HTTP request and handles response:
   * - next: Called on successful response
   * - error: Called on error response (401, 500, etc.)
   * 
   * NAVIGATION:
   * ----------
   * router.navigate(['/dashboard']) performs client-side navigation
   * - No page reload (SPA behavior)
   * - URL changes to /dashboard
   * - DashboardComponent is loaded
   */
  onSubmit(): void {
    this.errorMessage = '';  // Clear previous errors

    // Call authentication service
    this.authService.login({ email: this.email, password: this.password })
      .subscribe({
        next: () => {
          // Login successful - navigate to dashboard
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          // Login failed - show error message
          this.errorMessage = 'Invalid email or password';
        }
      });
  }
}
