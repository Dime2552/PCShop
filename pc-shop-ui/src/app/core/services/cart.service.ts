import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { CartResponseDto } from '../models/cart-response.model';

@Injectable({ providedIn: 'root' })
export class CartService {
  private http = inject(HttpClient);
  
  cartState = signal<CartResponseDto | null>(null);

  loadCart() {
    this.http.get<CartResponseDto>('https://localhost:7120/api/cart').subscribe(res => {
      this.cartState.set(res);
    });
  }

  addToCart(productId: string, quantity: number = 1) {
    return this.http.post('https://localhost:7120/api/cart/items', { productId, quantity });
  }
  
  updateQuantity(productId: string, quantity: number) {
    return this.http.put(`https://localhost:7120/api/cart/items/${productId}`, { quantity });
  }

  removeItem(productId: string) {
    return this.http.delete(`https://localhost:7120/api/cart/items/${productId}`);
  }

  clearCart() {
    return this.http.delete('https://localhost:7120/api/cart');
  }
}