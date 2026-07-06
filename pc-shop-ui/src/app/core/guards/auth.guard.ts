import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { MessageService } from 'primeng/api';

export const authGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    const messageService = inject(MessageService);

    if (authService.currentUser()) {
        return true;
    }

    // Redirect to auth if not logged in
    messageService.add({
        severity: 'warn',
        summary: 'Auth Required',
        detail: 'Please log in to proceed to checkout'
    });

    router.navigate(['/auth']);
    return false;
};