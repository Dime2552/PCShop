import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { AuthService } from '../../core/services/auth.service';
import { CartService } from '../../core/services/cart.service';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';

@Component({
  selector: 'app-auth',
  imports: [FormsModule, ButtonModule, InputTextModule, PasswordModule],
  templateUrl: './auth.html',
  styleUrl: './auth.css',
})
export class AuthComponent {
  private authService = inject(AuthService);
  private cartService = inject(CartService);
  private messageService = inject(MessageService);
  private router = inject(Router);

  isLoginMode = signal(true);

  // Form fields
  email = '';
  password = '';
  firstName = '';
  lastName = '';

  toggleMode() {
    this.isLoginMode.set(!this.isLoginMode());
  }

  onSubmit() {
    if (this.isLoginMode()) {
      this.authService.login({ email: this.email, password: this.password }).subscribe({
        next: () => this.onSuccess('Welcome back!'),
        error: (err) => this.onError(err)
      });
    } else {
      this.authService.register({
        email: this.email,
        password: this.password,
        firstName: this.firstName,
        lastName: this.lastName
      }).subscribe({
        next: () => this.onSuccess('Registration successful!'),
        error: (err) => this.onError(err)
      });
    }
  }

  private onSuccess(message: string) {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: message });
    // Reload cart because backend merged guest cart with user cart
    this.cartService.loadCart();
    this.router.navigate(['/']);
  }

  private onError(err: any) {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.detail || 'Authentication failed' });
  }
}