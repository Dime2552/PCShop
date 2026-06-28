import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { MessageService } from 'primeng/api';

export const adminGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    const messageService = inject(MessageService);

    const user = authService.currentUser();

    if (user && user.role === 'Admin') {
        return true;
    }

    messageService.add({ severity: 'error', summary: 'Access Denied', detail: 'Admin privileges required' });
    router.navigate(['/']);
    return false;
};