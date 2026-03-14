import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router);

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            // If 401 on any endpoint EXCEPT the session check itself,
            // clear storage and redirect to login
            if (error.status === 401 && !req.url.includes('/api/auth/session')) {
                localStorage.removeItem('user');
                router.navigate(['/login']);
            }
            // Always propagate the error so component error handlers fire (clears loading state)
            return throwError(() => error);
        })
    );
};
