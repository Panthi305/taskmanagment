import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TaskService } from '../../services/task.service';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user';

@Component({
  selector: 'app-create-task',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './create-task.html',
  styleUrl: './create-task.css'
})
export class CreateTaskComponent implements OnInit {
  title = '';
  description = '';
  assignedTo = 0;
  priority = 'Medium';
  users: User[] = [];
  successMessage = '';
  errorMessage = '';

  constructor(
    private taskService: TaskService,
    private userService: UserService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users.filter(u => u.role === 'Employee');
      },
      error: (error) => {
        this.errorMessage = 'Failed to load users';
      }
    });
  }

  onSubmit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (!this.title || !this.description || !this.assignedTo) {
      this.errorMessage = 'Please fill all fields';
      return;
    }

    this.taskService.createTask({
      title: this.title,
      description: this.description,
      assignedTo: this.assignedTo,
      priority: this.priority
    }).subscribe({
      next: () => {
        this.successMessage = 'Task created successfully!';
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
