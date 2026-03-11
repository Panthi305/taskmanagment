import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { DashboardComponent } from './components/dashboard/dashboard';
import { CreateTaskComponent } from './components/create-task/create-task';
import { TaskListComponent } from './components/task-list/task-list';
import { MyTasksComponent } from './components/my-tasks/my-tasks';

export const routes: Routes = [
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'dashboard', component: DashboardComponent },
    { path: 'create-task', component: CreateTaskComponent },
    { path: 'tasks', component: TaskListComponent },
    { path: 'my-tasks', component: MyTasksComponent }
];
