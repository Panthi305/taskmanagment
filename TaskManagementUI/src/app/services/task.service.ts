import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task, CreateTaskRequest } from '../models/task';

@Injectable({
    providedIn: 'root'
})
export class TaskService {
    private apiUrl = 'http://localhost:5150/api/task';

    constructor(private http: HttpClient) { }

    getTasks(): Observable<Task[]> {
        return this.http.get<Task[]>(this.apiUrl, { withCredentials: true });
    }

    getMyTasks(): Observable<Task[]> {
        return this.http.get<Task[]>(`${this.apiUrl}/my`, { withCredentials: true });
    }

    createTask(task: CreateTaskRequest): Observable<Task> {
        return this.http.post<Task>(this.apiUrl, task, { withCredentials: true });
    }

    startTask(id: number): Observable<any> {
        return this.http.put(`${this.apiUrl}/start/${id}`, {}, { withCredentials: true });
    }

    completeTask(id: number): Observable<any> {
        return this.http.put(`${this.apiUrl}/complete/${id}`, {}, { withCredentials: true });
    }
}
