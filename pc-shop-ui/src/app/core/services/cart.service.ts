import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { CartResponseDto } from '../models/cart-response.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CartService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/cart`;
  
  cartState = signal<CartResponseDto | null>(null);

  loadCart() {
    this.http.get<CartResponseDto>(this.apiUrl).subscribe(res => {
      this.cartState.set(res);
    });
  }

  addToCart(productId: string, quantity: number = 1) {
    return this.http.post(`${this.apiUrl}/items`, { productId, quantity });
  }
  
  updateQuantity(productId: string, quantity: number) {
    return this.http.put(`${this.apiUrl}/items/${productId}`, { quantity });
  }

  removeItem(productId: string) {
    return this.http.delete(`${this.apiUrl}/items/${productId}`);
  }

  clearCart() {
    return this.http.delete(this.apiUrl);
  }
}