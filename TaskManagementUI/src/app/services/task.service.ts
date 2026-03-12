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
     * GET COMPLETED TASKS - RETRIEVE COMPLETED TASKS
     * ========================================================================
     * 
     * Returns all completed tasks based on user role.
     */
    getCompletedTasks(): Observable<Task[]> {
        return this.http.get<Task[]>(`${this.apiUrl}/completed`, { withCredentials: true });
    }

    /*
     * ========================================================================
     * GET PENDING TASKS - RETRIEVE PENDING TASKS
     * ========================================================================
     * 
     * Returns all pending tasks based on user role.
     */
    getPendingTasks(): Observable<Task[]> {
        return this.http.get<Task[]>(`${this.apiUrl}/pending`, { withCredentials: true });
    }

    /*
     * ========================================================================
     * GET CREATED BY ME - RETRIEVE TASKS CREATED BY CURRENT USER
     * ========================================================================
     * 
     * Returns all tasks created by the current user (Admin/Manager only).
     */
    getCreatedByMe(): Observable<Task[]> {
        return this.http.get<Task[]>(`${this.apiUrl}/created-by-me`, { withCredentials: true });
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

    /*
     * ========================================================================
     * UPDATE TASK - EDIT TASK DETAILS (CREATOR ONLY, NOT COMPLETED)
     * ========================================================================
     * 
     * Only task creator can edit task.
     * Cannot edit completed tasks.
     */
    updateTask(id: number, task: any): Observable<Task> {
        return this.http.put<Task>(`${this.apiUrl}/${id}`, task, { withCredentials: true });
    }

    /*
     * ========================================================================
     * GET TASK BY ID - RETRIEVE TASK DETAILS
     * ========================================================================
     */
    getTaskById(id: number): Observable<Task> {
        return this.http.get<Task>(`${this.apiUrl}/${id}`, { withCredentials: true });
    }

    /*
     * ========================================================================
     * PROGRESS UPDATES
     * ========================================================================
     */
    addProgressUpdate(taskId: number, description: string, file?: File): Observable<any> {
        const formData = new FormData();
        formData.append('description', description);
        if (file) {
            formData.append('file', file);
        }
        return this.http.post(`${this.apiUrl}/${taskId}/progress`, formData, { withCredentials: true });
    }

    getProgressUpdates(taskId: number): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/${taskId}/progress`, { withCredentials: true });
    }

    /*
     * ========================================================================
     * ATTACHMENTS
     * ========================================================================
     */
    uploadAttachment(taskId: number, file: File): Observable<any> {
        const formData = new FormData();
        formData.append('file', file);
        return this.http.post(`${this.apiUrl}/${taskId}/attachments`, formData, { withCredentials: true });
    }

    getAttachments(taskId: number): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/${taskId}/attachments`, { withCredentials: true });
    }

    /*
     * ========================================================================
     * COMMENTS
     * ========================================================================
     */
    addComment(taskId: number, comment: string): Observable<any> {
        return this.http.post(`${this.apiUrl}/${taskId}/comments`, { comment }, { withCredentials: true });
    }

    getComments(taskId: number): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/${taskId}/comments`, { withCredentials: true });
    }

    /*
     * ========================================================================
     * ATTACHMENT PERMISSION REQUESTS
     * ========================================================================
     */
    createAttachmentPermissionRequest(taskId: number, requestType: string, message: string): Observable<any> {
        return this.http.post(`${this.apiUrl}/${taskId}/attachment-request`, { requestType, message }, { withCredentials: true });
    }

    getAttachmentPermissionRequests(taskId: number): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/${taskId}/attachment-requests`, { withCredentials: true });
    }

    approveAttachmentPermissionRequest(requestId: number): Observable<any> {
        return this.http.post(`${this.apiUrl}/attachment-request/${requestId}/approve`, {}, { withCredentials: true });
    }

    rejectAttachmentPermissionRequest(requestId: number): Observable<any> {
        return this.http.post(`${this.apiUrl}/attachment-request/${requestId}/reject`, {}, { withCredentials: true });
    }
}
