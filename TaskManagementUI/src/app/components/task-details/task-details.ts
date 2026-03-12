import { Component, OnInit } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskService } from '../../services/task.service';
import { AuthService } from '../../services/auth.service';
import { Task, TaskComment, TaskProgressUpdate, TaskAttachment } from '../../models/task';

@Component({
    selector: 'app-task-details',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './task-details.html',
    styleUrls: ['./task-details.css']
})
export class TaskDetailsComponent implements OnInit {
    task: Task | null = null;
    comments: TaskComment[] = [];
    progressUpdates: TaskProgressUpdate[] = [];
    attachments: TaskAttachment[] = [];

    newComment: string = '';
    newProgressDescription: string = '';
    selectedProgressFile: File | null = null;
    selectedAttachmentFile: File | null = null;

    // Edit mode
    isEditMode: boolean = false;
    editedTask: any = {};

    currentUserId: number = 0;
    currentUserRole: string = '';

    loading: boolean = true;
    error: string = '';

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private location: Location,
        private taskService: TaskService,
        private authService: AuthService
    ) { }

    ngOnInit(): void {
        const user = this.authService.getCurrentUser();
        if (user) {
            this.currentUserId = user.id;
            this.currentUserRole = user.role;
        }

        const taskId = Number(this.route.snapshot.paramMap.get('id'));
        if (taskId) {
            this.loadTaskDetails(taskId);
        }
    }

    loadTaskDetails(taskId: number): void {
        this.loading = true;
        this.error = '';

        this.taskService.getTaskById(taskId).subscribe({
            next: (task) => {
                this.task = task;
                this.loadComments(taskId);
                this.loadProgressUpdates(taskId);
                this.loadAttachments(taskId);
                this.loading = false;
            },
            error: (err) => {
                console.error('Failed to load task details', err);
                this.error = 'Task details could not be loaded. The task may not exist or you may not have permission to view it.';
                this.loading = false;
            }
        });
    }

    loadComments(taskId: number): void {
        this.taskService.getComments(taskId).subscribe({
            next: (comments) => {
                this.comments = comments;
            },
            error: (err) => {
                console.error('Failed to load comments', err);
            }
        });
    }

    loadProgressUpdates(taskId: number): void {
        this.taskService.getProgressUpdates(taskId).subscribe({
            next: (updates) => {
                this.progressUpdates = updates;
            },
            error: (err) => {
                console.error('Failed to load progress updates', err);
            }
        });
    }

    loadAttachments(taskId: number): void {
        this.taskService.getAttachments(taskId).subscribe({
            next: (attachments) => {
                this.attachments = attachments;
            },
            error: (err) => {
                console.error('Failed to load attachments', err);
            }
        });
    }

    addComment(): void {
        if (!this.task || !this.newComment.trim()) return;

        this.taskService.addComment(this.task.id, this.newComment).subscribe({
            next: (comment) => {
                this.comments.push(comment);
                this.newComment = '';
            },
            error: (err) => {
                alert('Failed to add comment');
            }
        });
    }

    addProgressUpdate(): void {
        if (!this.task || !this.newProgressDescription.trim()) return;

        this.taskService.addProgressUpdate(
            this.task.id,
            this.newProgressDescription,
            this.selectedProgressFile || undefined
        ).subscribe({
            next: (update) => {
                this.progressUpdates.unshift(update);
                this.newProgressDescription = '';
                this.selectedProgressFile = null;
            },
            error: (err) => {
                alert('Failed to add progress update: ' + (err.error?.message || 'Unknown error'));
            }
        });
    }

    uploadAttachment(): void {
        if (!this.task || !this.selectedAttachmentFile) return;

        this.taskService.uploadAttachment(this.task.id, this.selectedAttachmentFile).subscribe({
            next: (attachment) => {
                this.attachments.unshift(attachment);
                this.selectedAttachmentFile = null;
            },
            error: (err) => {
                alert('Failed to upload attachment');
            }
        });
    }

    onProgressFileSelected(event: any): void {
        this.selectedProgressFile = event.target.files[0];
    }

    onAttachmentFileSelected(event: any): void {
        this.selectedAttachmentFile = event.target.files[0];
    }

    startTask(): void {
        if (!this.task) return;

        this.taskService.startTask(this.task.id).subscribe({
            next: () => {
                this.loadTaskDetails(this.task!.id);
            },
            error: (err) => {
                alert('Failed to start task');
            }
        });
    }

    completeTask(): void {
        if (!this.task) return;

        this.taskService.completeTask(this.task.id).subscribe({
            next: () => {
                this.loadTaskDetails(this.task!.id);
            },
            error: (err) => {
                alert('Failed to complete task');
            }
        });
    }

    canModifyTask(): boolean {
        return this.task?.assignedTo === this.currentUserId;
    }

    canUploadAttachment(): boolean {
        if (!this.task) return false;

        // COMPLETED TASK LOCK RULE: No one can upload to completed tasks
        if (this.task.status === 'Completed') {
            return false;
        }

        // Only task creator can upload
        if (this.task.assignedBy !== this.currentUserId) {
            return false;
        }

        return true;
    }

    canEditTask(): boolean {
        if (!this.task) return false;

        // COMPLETED TASK LOCK RULE: No one can edit completed tasks
        if (this.task.status === 'Completed') {
            return false;
        }

        // Only task creator can edit active tasks
        return this.task.assignedBy === this.currentUserId;
    }

    canAddProgress(): boolean {
        if (!this.task) return false;

        // COMPLETED TASK LOCK RULE: No one can add progress to completed tasks
        if (this.task.status === 'Completed') {
            return false;
        }

        // Only assigned user can add progress
        return this.task.assignedTo === this.currentUserId;
    }

    isTaskCompleted(): boolean {
        return this.task?.status === 'Completed';
    }

    canSeePermissionRequests(): boolean {
        if (!this.task) return false;

        // Only task creator can see permission requests
        return this.task.assignedBy === this.currentUserId;
    }

    enableEditMode(): void {
        if (!this.task) return;

        this.isEditMode = true;
        this.editedTask = {
            title: this.task.title,
            description: this.task.description,
            priority: this.task.priority,
            deadline: this.task.deadline
        };
    }

    cancelEdit(): void {
        this.isEditMode = false;
        this.editedTask = {};
    }

    saveTask(): void {
        if (!this.task) return;

        this.taskService.updateTask(this.task.id, this.editedTask).subscribe({
            next: (updatedTask) => {
                this.task = updatedTask;
                this.isEditMode = false;
                this.editedTask = {};
                alert('Task updated successfully');
            },
            error: (err) => {
                alert('Failed to update task: ' + (err.error?.message || 'Unknown error'));
            }
        });
    }

    goBack(): void {
        this.location.back();
    }
}
