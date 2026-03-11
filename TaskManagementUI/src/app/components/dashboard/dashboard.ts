import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TaskService } from '../../services/task.service';
import { User } from '../../models/user';
import { Task } from '../../models/task';

/*
 * ============================================================================
 * DASHBOARD COMPONENT - MAIN APPLICATION DASHBOARD
 * ============================================================================
 * 
 * COMPONENT LIFECYCLE:
 * -------------------
 * Angular components have lifecycle hooks:
 * 
 * 1. constructor(): Called when component is created
 *    - Initialize services
 *    - Don't perform heavy operations here
 * 
 * 2. ngOnInit(): Called after component is initialized
 *    - Fetch data from APIs
 *    - Set up subscriptions
 *    - Initialize component state
 * 
 * 3. ngOnDestroy(): Called before component is destroyed
 *    - Clean up subscriptions
 *    - Release resources
 * 
 * IMPLEMENTS OnInit:
 * -----------------
 * - Interface that requires ngOnInit() method
 * - Best practice for initialization logic
 * - Separates construction from initialization
 * 
 * COMPONENT COMMUNICATION:
 * -----------------------
 * This component communicates with:
 * - AuthService: Get current user, check authentication
 * - TaskService: Fetch tasks, get statistics
 * - Router: Navigate to other pages
 * 
 * REACTIVE PROGRAMMING:
 * --------------------
 * Subscribes to currentUser$ observable:
 * - Automatically updates when auth state changes
 * - Reloads tasks when user changes
 * - Redirects to login if user logs out
 */

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],  // RouterModule for routerLink directive
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {
  // Component state
  currentUser: User | null = null;
  tasks: Task[] = [];
  stats = {
    total: 0,
    pending: 0,
    inProgress: 0,
    completed: 0
  };

  constructor(
    private authService: AuthService,
    private taskService: TaskService,
    private router: Router
  ) { }

  /*
   * ==========================================================================
   * NG ON INIT - COMPONENT INITIALIZATION
   * ==========================================================================
   * 
   * Called once after component is created and inputs are set.
   * 
   * OBSERVABLE SUBSCRIPTION:
   * -----------------------
   * Subscribes to currentUser$ observable from AuthService.
   * Whenever auth state changes, this callback executes:
   * - User logs in: Load tasks
   * - User logs out: Redirect to login
   * - Page refresh: Restore state from session
   * 
   * This creates reactive UI that responds to auth state changes.
   */
  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      if (!user) {
        // Not authenticated - redirect to login
        this.router.navigate(['/login']);
      } else {
        // Authenticated - load tasks
        this.loadTasks();
      }
    });
  }

  /*
   * ==========================================================================
   * LOAD TASKS - FETCH TASKS FROM API
   * ==========================================================================
   * 
   * ROLE-BASED DATA LOADING:
   * ------------------------
   * - Employee: Calls getMyTasks() - only their assigned tasks
   * - Admin/Manager: Calls getTasks() - all tasks they can see
   * 
   * After loading tasks, calculates statistics for dashboard display.
   */
  loadTasks(): void {
    if (this.currentUser?.role === 'Employee') {
      // Employee sees only their tasks
      this.taskService.getMyTasks().subscribe(tasks => {
        this.tasks = tasks;
        this.calculateStats();
      });
    } else {
      // Admin/Manager see filtered tasks
      this.taskService.getTasks().subscribe(tasks => {
        this.tasks = tasks;
        this.calculateStats();
      });
    }
  }

  /*
   * ==========================================================================
   * CALCULATE STATS - COMPUTE TASK STATISTICS
   * ==========================================================================
   * 
   * ARRAY FILTERING:
   * ---------------
   * Uses JavaScript array.filter() method:
   * - filter(t => t.status === 'Pending') returns array of pending tasks
   * - .length gives count of matching tasks
   * 
   * This demonstrates client-side data processing.
   * Alternative: Backend could provide these statistics.
   */
  calculateStats(): void {
    this.stats.total = this.tasks.length;
    this.stats.pending = this.tasks.filter(t => t.status === 'Pending').length;
    this.stats.inProgress = this.tasks.filter(t => t.status === 'In Progress').length;
    this.stats.completed = this.tasks.filter(t => t.status === 'Completed').length;
  }

  /*
   * ==========================================================================
   * LOGOUT - END USER SESSION
   * ==========================================================================
   * 
   * Calls AuthService.logout() and navigates to login page.
   */
  logout(): void {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }

  /*
   * ==========================================================================
   * CAN CREATE TASK - CHECK IF USER CAN CREATE TASKS
   * ==========================================================================
   * 
   * CONDITIONAL RENDERING:
   * ---------------------
   * Used in template with *ngIf directive:
   * <button *ngIf="canCreateTask()">Create Task</button>
   * 
   * Button only appears for Admin and Manager roles.
   * This is client-side authorization for UI rendering.
   * Backend still enforces authorization for API calls.
   */
  canCreateTask(): boolean {
    return this.currentUser?.role === 'Admin' || this.currentUser?.role === 'Manager';
  }
}
