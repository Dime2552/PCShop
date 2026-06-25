import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { AuthResponseDto, LoginRequest, RegisterRequest } from '../models/auth.model';
import { environment } from '../../../environments/environment';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = `${environment.apiUrl}/auth`;

  // Signal to hold current user state
  currentUser = signal<AuthResponseDto | null>(null);

  constructor() {
    // Restore user from localStorage on app start
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      this.currentUser.set(JSON.parse(storedUser));
    }
  }

  login(data: LoginRequest) {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, data).pipe(
      tap(response => this.handleAuthSuccess(response))
    );
  }

  register(data: RegisterRequest) {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/register`, data).pipe(
      tap(response => this.handleAuthSuccess(response))
    );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.currentUser.set(null);
    this.router.navigate(['/']);
  }

  private handleAuthSuccess(response: AuthResponseDto) {
    localStorage.setItem('token', response.token);
    localStorage.setItem('user', JSON.stringify(response));
    this.currentUser.set(response);
  }
}