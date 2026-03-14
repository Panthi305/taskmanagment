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

    // Edit request
    showEditRequestDialog: boolean = false;
    editRequestMessage: string = '';
    editRequests: any[] = [];
    hasApprovedEditRequest: boolean = false;
    hasPendingEditRequest: boolean = false;
    hasRejectedEditRequest: boolean = false;

    currentUserId: number = 0;
    currentUserRole: string = '';
    backToTasksRoute: string = '/tasks';

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
            this.backToTasksRoute = this.currentUserRole.toLowerCase() === 'employee' ? '/my-tasks' : '/tasks';
        }

        this.route.paramMap.subscribe(params => {
            const rawId = params.get('id');
            const taskId = rawId ? Number(rawId) : NaN;

            if (!rawId || Number.isNaN(taskId) || taskId <= 0) {
                this.error = 'Invalid task id.';
                this.loading = false;
                return;
            }

            this.loadTaskDetails(taskId);
        });
    }

    loadTaskDetails(taskId: number): void {
        this.loading = true;
        this.error = '';

        // Fetch the task first so the UI can render quickly.
        this.taskService.getTaskById(taskId).subscribe({
            next: (task) => {
                this.task = task;
                this.loading = false;

                const isCreator = task.assignedBy === this.currentUserId;

                // Load the rest in parallel without blocking the main view.
                this.loadComments(taskId);
                this.loadProgressUpdates(taskId);
                this.loadAttachments(taskId);

                if (isCreator) {
                    this.loadEditRequests(taskId);
                } else {
                    this.checkEditRequestStatus(taskId);
                }
            },
            error: () => {
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

        // Task creator can always edit
        if (this.task.assignedBy === this.currentUserId) {
            return true;
        }

        // User with approved edit request can edit
        return this.hasApprovedEditRequest;
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

    goBackToTasks(): void {
        this.router.navigateByUrl(this.backToTasksRoute);
    }

    // ========================================================================
    // EDIT REQUEST METHODS
    // ========================================================================

    canRequestEditAccess(): boolean {
        if (!this.task) return false;

        // Cannot request if you're the creator
        if (this.task.assignedBy === this.currentUserId) {
            return false;
        }

        // Only the assigned user can request edit access
        if (this.task.assignedTo !== this.currentUserId) {
            return false;
        }
        // Cannot request if task is completed
        if (this.task.status === 'Completed') {
            return false;
        }

        // Cannot request if already approved
        if (this.hasApprovedEditRequest) {
            return false;
        }

        // Cannot request if already pending
        if (this.hasPendingEditRequest) {
            return false;
        }

        // Cannot request if already rejected
        if (this.hasRejectedEditRequest) {
            return false;
        }

        return true;
    }

    canSeeEditRequests(): boolean {
        if (!this.task) return false;

        // Only task creator can see edit requests
        return this.task.assignedBy === this.currentUserId;
    }

    checkEditRequestStatus(taskId: number): void {
        // Skip if user is the task creator â€” they can always edit
        if (!this.task || this.task.assignedBy === this.currentUserId) {
            return;
        }

        // Only assigned user should check their edit request status
        if (this.task.assignedTo !== this.currentUserId) {
            return;
        }
        // Reset flags before checking
        this.hasApprovedEditRequest = false;
        this.hasPendingEditRequest = false;
        this.hasRejectedEditRequest = false;

        this.taskService.getEditRequests(taskId).subscribe({
            next: (requests) => {
                // Backend returns only this user's own request(s)
                if (requests && requests.length > 0) {
                    const myRequest = requests[0]; // Only one request per user per task
                    this.hasApprovedEditRequest = myRequest.status === 'Approved';
                    this.hasPendingEditRequest = myRequest.status === 'Pending';
                    this.hasRejectedEditRequest = myRequest.status === 'Rejected';
                }
            },
            error: () => {
                // Silently fail â€” user simply has no edit access
            }
        });
    }

    loadEditRequests(taskId: number): void {
        this.taskService.getEditRequests(taskId).subscribe({
            next: (requests) => {
                this.editRequests = requests;
            },
            error: (err) => {
                console.error('Failed to load edit requests', err);
            }
        });
    }

    openEditRequestDialog(): void {
        this.showEditRequestDialog = true;
        this.editRequestMessage = '';
    }

    closeEditRequestDialog(): void {
        this.showEditRequestDialog = false;
        this.editRequestMessage = '';
    }

    submitEditRequest(): void {
        if (!this.task) return;

        if (this.task.assignedTo !== this.currentUserId) {
            alert('Only the user assigned to this task can request edit access.');
            return;
        }
        this.taskService.createEditRequest(this.task.id, this.editRequestMessage).subscribe({
            next: () => {
                this.closeEditRequestDialog();
                // Reload status from server to reflect the new pending state
                this.checkEditRequestStatus(this.task!.id);
            },
            error: (err) => {
                alert('Failed to submit edit request: ' + (err.error?.message || 'Unknown error'));
            }
        });
    }

    approveEditRequest(requestId: number): void {
        this.taskService.approveEditRequest(requestId).subscribe({
            next: () => {
                if (this.task) {
                    this.loadEditRequests(this.task.id);
                }
            },
            error: (err) => {
                alert('Failed to approve request: ' + (err.error?.message || 'Unknown error'));
            }
        });
    }

    rejectEditRequest(requestId: number): void {
        this.taskService.rejectEditRequest(requestId).subscribe({
            next: () => {
                if (this.task) {
                    this.loadEditRequests(this.task.id);
                }
            },
            error: (err) => {
                alert('Failed to reject request: ' + (err.error?.message || 'Unknown error'));
            }
        });
    }
}




