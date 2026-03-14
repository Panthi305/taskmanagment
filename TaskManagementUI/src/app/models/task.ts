export interface Task {
    id: number;
    title: string;
    description: string;
    assignedBy: number;
    assignedByName: string;
    assignedTo: number;
    assignedToName: string;
    priority: string;
    status: string;
    createdAt: Date;
    startDate?: Date;
    completedDate?: Date;
    deadline?: Date;
}

export interface CreateTaskRequest {
    title: string;
    description: string;
    assignedTo: number;
    priority: string;
    deadline?: Date;
}

export interface TaskComment {
    id: number;
    taskId: number;
    userId: number;
    userName: string;
    comment: string;
    createdAt: Date;
}

export interface CreateCommentRequest {
    comment: string;
}

export interface TaskProgressUpdate {
    id: number;
    taskId: number;
    userId: number;
    userName: string;
    description: string;
    filePath?: string;
    fileUrl?: string;
    createdAt: Date;
}

export interface TaskAttachment {
    id: number;
    taskId: number;
    fileName: string;
    filePath: string;
    uploadedBy: number;
    uploadedByName: string;
    uploadedAt: Date;
}
