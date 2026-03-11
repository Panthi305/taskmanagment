import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TaskService } from '../../services/task.service';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user';

/*
 * ============================================================================
 * CREATE TASK COMPONENT - FORM FOR CREATING NEW TASKS
 * ============================================================================
 * 
 * This component demonstrates:
 * - Template-driven forms with validation
 * - Two-way data binding with [(ngModel)]
 * - Dropdown selection binding
 * - Form submission handling
 * - Client-side validation
 * - Success/error message display
 * - Programmatic navigation after success
 * 
 * FORM WORKFLOW:
 * -------------
 * 1. Component loads → ngOnInit() fetches employee list
 * 2. User fills form fields (bound via ngModel)
 * 3. User selects employee from dropdown
 * 4. User clicks submit → onSubmit() validates and sends data
 * 5. Success → Show message and redirect to dashboard
 * 6. Error → Show error message, stay on form
 * 
 * ANGULAR FORMS:
 * -------------
 * Template-driven forms use:
 * - FormsModule for ngModel directive
 * - Two-way binding: [(ngModel)]="property"
 * - Form validation in template or component
 * - Simple for basic forms
 * 
 * Alternative: Reactive Forms
 * - More powerful and testable
 * - Better for complex forms
 * - Validation in component class
 */

@Component({
  selector: 'app-create-task',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './create-task.html',
  styleUrl: './create-task.css'
})
export class CreateTaskComponent implements OnInit {
  // Form fields - bound to template inputs via ngModel
  title = '';
  description = '';
  assignedTo = 0;
  priority = 'Medium';

  // Data for dropdown
  users: User[] = [];

  // UI feedback messages
  successMessage = '';
  errorMessage = '';

  constructor(
    private taskService: TaskService,
    private userService: UserService,
    private router: Router
  ) { }

  /*
   * ==========================================================================
   * NG ON INIT - LOAD EMPLOYEE LIST
   * ==========================================================================
   * 
   * Fetches all users and filters to show only employees.
   * Employees are the only ones who can be assigned tasks.
   */
  ngOnInit(): void {
    this.loadUsers();
  }

  /*
   * ==========================================================================
   * LOAD USERS - FETCH AND FILTER EMPLOYEES
   * ==========================================================================
   * 
   * ARRAY FILTERING:
   * ---------------
   * users.filter(u => u.role === 'Employee')
   * 
   * Returns new array containing only users with Employee role.
   * This filtered list populates the "Assign To" dropdown.
   */
  loadUsers(): void {
    this.userService.getUsers().subscribe({
      next: (users) => {
        // Filter to show only employees in dropdown
        this.users = users.filter(u => u.role === 'Employee');
      },
      error: (error) => {
        this.errorMessage = 'Failed to load users';
      }
    });
  }

  /*
   * ==========================================================================
   * ON SUBMIT - HANDLE FORM SUBMISSION
   * ==========================================================================
   * 
   * VALIDATION:
   * ----------
   * Client-side validation checks:
   * - Title is not empty
   * - Description is not empty
   * - Employee is selected (assignedTo > 0)
   * 
   * If validation fails, shows error and returns early.
   * 
   * SUCCESS HANDLING:
   * ----------------
   * - Shows success message
   * - Waits 1.5 seconds (user can see message)
   * - Navigates to dashboard
   * 
   * setTimeout() delays navigation so user sees success message.
   */
  onSubmit(): void {
    // Clear previous messages
    this.successMessage = '';
    this.errorMessage = '';

    // Client-side validation
    if (!this.title || !this.description || !this.assignedTo) {
      this.errorMessage = 'Please fill all fields';
      return;
    }

    // Create task via service
    this.taskService.createTask({
      title: this.title,
      description: this.description,
      assignedTo: this.assignedTo,
      priority: this.priority
    }).subscribe({
      next: () => {
        this.successMessage = 'Task created successfully!';
        // Delay navigation to show success message
        setTimeout(() => {
          this.router.navigate(['/dashboard']);
        }, 1500);
      },
      error: (error) => {
        this.errorMessage = 'Failed to create task';
      }
    });
  }
}
