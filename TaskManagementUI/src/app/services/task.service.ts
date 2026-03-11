import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task, CreateTaskRequest } from '../models/task';

/*
 * ============================================================================
 * TASK SERVICE - ANGULAR SERVICE FOR TASK MANAGEMENT
 * ============================================================================
 * 
 * This service handles all task-related API communication.
 * 
 * SERVICE RESPONSIBILITIES:
 * ------------------------
 * - Encapsulate HTTP communication logic
 * - Provide clean API for components
 * - Handle data transformation if needed
 * - Centralize API endpoint URLs
 * - Enable easy testing (can mock service)
 * 
 * SEPARATION OF CONCERNS:
 * ----------------------
 * Components focus on:
 * - User interface
 * - User interactions
 * - Display logic
 * 
 * Services focus on:
 * - Data fetching
 * - Business logic
 * - State management
 * 
 * This separation makes code more maintainable and testable.
 */

@Injectable({
    providedIn: 'root'
})
export class TaskService {
    private apiUrl = 'http://localhost:5150/api/task';

    constructor(private http: HttpClient) { }

    /*
     * ========================================================================
     * GET TASKS - RETRIEVE TASKS BASED ON USER ROLE
     * ========================================================================
     * 
     * Backend automatically filters based on role:
     * - Admin: All tasks
     * - Manager: Tasks created by manager
     * - Employee: Tasks assigned to employee
     * 
     * Returns Observable<Task[]> - array of tasks
     */
    getTasks(): Observable<Task[]> {
        return this.http.get<Task[]>(this.apiUrl, { withCredentials: true });
    }

    /*
     * ========================================================================
     * GET MY TASKS - RETRIEVE TASKS ASSIGNED TO CURRENT USER
     * ========================================================================
     * 
     * Specifically for employees to view their assigned tasks.
     */
    getMyTasks(): Observable<Task[]> {
        return this.http.get<Task[]>(`${this.apiUrl}/my`, { withCredentials: true });
    }

    /*
     * ========================================================================
     * CREATE TASK - CREATE NEW TASK (ADMIN/MANAGER ONLY)
     * ========================================================================
     * 
     * Sends CreateTaskRequest DTO to backend.
     * Backend validates role and creates task.
     */
    createTask(task: CreateTaskRequest): Observable<Task> {
        return this.http.post<Task>(this.apiUrl, task, { withCredentials: true });
    }

    /*
     * ========================================================================
     * START TASK - CHANGE STATUS FROM PENDING TO IN PROGRESS
     * ========================================================================
     * 
     * Only assigned employee can start their task.
     * Backend validates ownership and current status.
     */
    startTask(id: number): Observable<any> {
        return this.http.put(`${this.apiUrl}/start/${id}`, {}, { withCredentials: true });
    }

    /*
     * ========================================================================
     * COMPLETE TASK - CHANGE STATUS FROM IN PROGRESS TO COMPLETED
     * ========================================================================
     * 
     * Only assigned employee can complete their task.
     * Backend validates ownership and current status.
     */
    completeTask(id: number): Observable<any> {
        return this.http.put(`${this.apiUrl}/complete/${id}`, {}, { withCredentials: true });
    }
}
