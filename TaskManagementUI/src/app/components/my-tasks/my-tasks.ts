import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import { Task } from '../../models/task';

@Component({
  selector: 'app-my-tasks',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './my-tasks.html',
  styleUrl: './my-tasks.css'
})
export class MyTasksComponent implements OnInit {
  tasks: Task[] = [];
  filteredTasks: Task[] = [];
  filterStatus = 'All';

  constructor(private taskService: TaskService) { }

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getMyTasks().subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.applyFilter();
      },
      error: (error) => {
        console.error('Failed to load tasks', error);
      }
    });
  }

  applyFilter(): void {
    if (this.filterStatus === 'All') {
      this.filteredTasks = this.tasks;
    } else {
      this.filteredTasks = this.tasks.filter(t => t.status === this.filterStatus);
    }
  }

  startTask(taskId: number): void {
    this.taskService.startTask(taskId).subscribe({
      next: () => {
        this.loadTasks();
      },
      error: (error) => {
        alert('Failed to start task');
      }
    });
  }

  completeTask(taskId: number): void {
    this.taskService.completeTask(taskId).subscribe({
      next: () => {
        this.loadTasks();
      },
      error: (error) => {
        alert('Failed to complete task');
      }
    });
  }

  canStart(task: Task): boolean {
    return task.status === 'Pending';
  }

  canComplete(task: Task): boolean {
    return task.status === 'In Progress';
  }
}
