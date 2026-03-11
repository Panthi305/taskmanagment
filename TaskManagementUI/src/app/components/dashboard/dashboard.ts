import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TaskService } from '../../services/task.service';
import { User } from '../../models/user';
import { Task } from '../../models/task';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {
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

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      if (!user) {
        this.router.navigate(['/login']);
      } else {
        this.loadTasks();
      }
    });
  }

  loadTasks(): void {
    if (this.currentUser?.role === 'Employee') {
      this.taskService.getMyTasks().subscribe(tasks => {
        this.tasks = tasks;
        this.calculateStats();
      });
    } else {
      this.taskService.getTasks().subscribe(tasks => {
        this.tasks = tasks;
        this.calculateStats();
      });
    }
  }

  calculateStats(): void {
    this.stats.total = this.tasks.length;
    this.stats.pending = this.tasks.filter(t => t.status === 'Pending').length;
    this.stats.inProgress = this.tasks.filter(t => t.status === 'In Progress').length;
    this.stats.completed = this.tasks.filter(t => t.status === 'Completed').length;
  }

  logout(): void {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }

  canCreateTask(): boolean {
    return this.currentUser?.role === 'Admin' || this.currentUser?.role === 'Manager';
  }
}
