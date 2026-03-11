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
}

export interface CreateTaskRequest {
    title: string;
    description: string;
    assignedTo: number;
    priority: string;
}
