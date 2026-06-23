import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CartService } from '../../core/services/cart.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [RouterLink, ButtonModule],
  templateUrl: './cart.html'
})
export class CartComponent {
  cartService = inject(CartService);

  increaseQuantity(productId: string, currentQty: number) {
    this.updateQuantity(productId, currentQty + 1);
  }

  decreaseQuantity(productId: string, currentQty: number) {
    if (currentQty > 1) {
      this.updateQuantity(productId, currentQty - 1);
    }
  }

  updateQuantity(productId: string, quantity: number) {
    this.cartService.updateQuantity(productId, quantity).subscribe({
      next: () => this.cartService.loadCart(),
      error: (err) => alert(err.error?.detail || 'Can`t update quantity. Maybe, no more on storage.')
    });
  }

  removeItem(productId: string) {
    this.cartService.removeItem(productId).subscribe(() => {
      this.cartService.loadCart();
    });
  }

  clearCart() {
    if (confirm('Clear cart?')) {
      this.cartService.clearCart().subscribe(() => {
        this.cartService.loadCart();
      });
    }
  }
}